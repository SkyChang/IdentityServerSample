using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServerSample.Service
{
    public class Config
    {
        /// <summary>
        /// 定義資源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        /// <summary>
        /// 定義 Client
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    // 此種情境代表不需要多個 User 的身分，可能是簡單的 API <--> API 的關係
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                },

                // resource owner password grant client
                // 搭配底下的 TestUser 使用，直接提供帳號密碼給 Client 使用
                // 通常可能會有多個帳號，用於信任的應用，或是傳統的應用
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                },

                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5021/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5021/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

                // Hybrid Flow , 使用 OIDC + OAUTH,
                // 通常使用在 Client 為 Web , 並且需要 Web ( backend ) call backend service 的情境
                // 主要是先透過 OIDC 取得身分驗證後，在一次的交換需要的 Token ( OAUTH - Server 2 Server 用 )
                new Client
                {
                    ClientId = "mvc2",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    RedirectUris           = { "https://localhost:7001/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
                    // 我們還讓 Client 允許 offline_access - 這允許請求刷新令牌以實現長期 API 訪問
                    AllowOfflineAccess = true
                }


        };
        }

        /// <summary>
        /// 定義 User，這種模式，通常提供給受信任的 Client，並直接給予帳號密碼
        /// 通常帳號可能是 Guid，密碼可能是一連串的數字，通常， Google API 等等，
        /// 會用此模式。
        /// </summary>
        /// <returns></returns>
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",
                    // 除了預設，可以添加一些新的 Claim，讓 Client 抓到與使用
                    Claims = new []
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",
                    Claims = new []
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }

        /// <summary>
        /// OIDC 允許資源設定，這邊 OIDC 允許 Client 可以拿到怎樣的使用者資源 ( 預設為 OpenId 和 Profile )
        /// 和上面的 OAUTH 不同，OAUTH 是設定可以存取那些 API
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

    }
}
