using System.Threading.Tasks;

namespace Front_End
{
    // This is a helper class which manages API calls to the backend related to Users.
    class BackendUser : Backend
    {
        public BackendUser() : base()
        {
            _Resource = "User";
        }

        public async Task Login(string username, string password)
        {
            _Command = "Login";

            // Set the parameters and post the login request.
            _Parameters.Add(new BackendParameter { Key = "username", Value = username });
            _Parameters.Add(new BackendParameter { Key = "password", Value = password });

            await PostRequestAsync();
        }

        public async Task Logout()
        {
            _Command = "Logout";

            await PostRequestAsync();
        }

        public async Task Register(string firstname, string lastname, string username, string password)
        {
            _Command = "Register";

            // Set the parameters and post the login request.
            _Parameters.Add(new BackendParameter { Key = "firstname", Value = firstname });
            _Parameters.Add(new BackendParameter { Key = "lastname", Value = lastname });
            _Parameters.Add(new BackendParameter { Key = "username", Value = username });
            _Parameters.Add(new BackendParameter { Key = "password", Value = password });

            await PostRequestAsync();
        }
    }
}