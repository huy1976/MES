namespace MesMicroservice.Application.Queries.Users
{
    public class CheckUserSignInQueryHandler : IRequestHandler<CheckUserSignInQuery, UserCheckDTO>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public CheckUserSignInQueryHandler(IUserRepository userRepository, IPersonRepository personRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public async Task<UserCheckDTO> Handle(CheckUserSignInQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByUsername(request.UserName);
            if (user is null)
            {
                throw new EntityNotFoundException(nameof(User), request.UserName);
            }

            bool verified = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!verified)
            {
                throw new Exception("Password: Incorrect.");
            }

            //var person = await _personRepository.GetAsync(user.Person.ResourceId);
            //if (person is null)
            //{
            //    throw new EntityNotFoundException(nameof(Person), user.Person.ResourceId);
            //}

            var userDTO = new UserCheckDTO(userId: user.UserId,
                                           userName: user.UserName,
                                           role: user.Role);

            return userDTO;
        }
    }
}
