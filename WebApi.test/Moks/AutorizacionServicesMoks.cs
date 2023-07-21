using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.test.Moks
{
    internal class AutorizacionServicesMoks : IAuthorizationService
    {
        //esto de aca es para poder facilitar cuando sea admin o no
        /*
         de esta forma
        cuando es admin
        var autorisado = new AutorizacionServicesMoks();
            autorisado.resultado = AuthorizationResult.Success();

        o cuando no sea admin
         var autorisado = new AutorizacionServicesMoks();
            autorisado.resultado = AuthorizationResult.Failed();
         
         */
        public AuthorizationResult resultado { get; set; }
        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(resultado);
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(resultado);
        }
    }
}
