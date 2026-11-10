using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesMicroservice.Application.Commands.Persons
{
    public class CreatePersonsCommand : IRequest<bool>
    {
        public List<CreatePersonCommand> createPersonCommands { get; set; } = new List<CreatePersonCommand>();

    }
}
