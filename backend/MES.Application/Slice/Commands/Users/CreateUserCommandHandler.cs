namespace MesMicroservice.Application.Commands.Users
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;

        public CreateUserCommandHandler(IUserRepository userRepository, IPersonRepository personRepository)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userCheck = await _userRepository.GetUserByUsername(request.UserName);
            if (userCheck is not null)
            {
                throw new EntityDuplicationException(nameof(User), request.UserName);
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User(userId: Guid.NewGuid().ToString(),
                    userName: request.UserName,
                    password: passwordHash,
                    role: request.Role);

            if (string.IsNullOrEmpty(request.PersonId))
            {
                _userRepository.Create(user);
                return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            var person = await _personRepository.GetAsync(request.PersonId);
            if (person is not null)
            {
                user.addPerson(person);
            }

            _userRepository.Create(user);


            return await _userRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
