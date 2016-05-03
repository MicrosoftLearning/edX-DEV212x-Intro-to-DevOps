using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PartsUnlimited.UnitTests.Fakes
{
    public class MockHttpContext : HttpContextBase
    {
        private Mock<HttpRequestBase> fakeRequest;
        private readonly IPrincipal _user;

        public MockHttpContext(string username = "bob", NameValueCollection queryString = null)
        {
            _user = new GenericPrincipal(new GenericIdentity(username), null /* roles */);

            fakeRequest = new Mock<HttpRequestBase>();
            fakeRequest.Setup(r => r.Headers).Returns(
                new WebHeaderCollection()
                {
                    { "X-Requested-With", "XMLHttpRequest" }
                }
            );
            fakeRequest.Setup(r => r.QueryString).Returns(queryString);
        }


        public override IPrincipal User
        {
            get
            {
                return _user;
            }
            set
            {
                base.User = value;
            }
        }

        public override HttpRequestBase Request
        {
            get
            {
                return fakeRequest.Object;
            }
        }
    }
}
