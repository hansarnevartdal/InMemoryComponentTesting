using System.Collections.Generic;

namespace ComponentBoundaries.Testing.Http.Auth0.Models
{
    public class Key
    {
        public string Alg { get; set; }
        public string Kty { get; set; }
        public string Use { get; set; }
        public List<string> X5C { get; set; }
        public string N { get; set; }
        public string E { get; set; }
        public string Kid { get; set; }
        public string X5T { get; set; }
    }
}