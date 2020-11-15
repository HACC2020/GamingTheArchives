using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ArchiveSiteBackend.Api.Middleware {
    public abstract class RequestMiddlewareBase<TDep> {
        private readonly RequestDelegate nextRequestDelegate;

        protected RequestMiddlewareBase(RequestDelegate nextRequestDelegate) {
            this.nextRequestDelegate =
                nextRequestDelegate ?? throw new ArgumentNullException(nameof(nextRequestDelegate));
        }

        public async Task InvokeAsync(HttpContext context, TDep scopedDependency) {
            await this.OnInvoke(context, scopedDependency);
            await this.nextRequestDelegate(context);
        }

        protected abstract Task OnInvoke(HttpContext context, TDep scopedDependency);
    }

    public abstract class RequestMiddlewareBase<TDep1, TDep2> {
        private readonly RequestDelegate nextRequestDelegate;

        protected RequestMiddlewareBase(RequestDelegate nextRequestDelegate) {
            this.nextRequestDelegate =
                nextRequestDelegate ?? throw new ArgumentNullException(nameof(nextRequestDelegate));
        }

        public async Task InvokeAsync(
            HttpContext context,
            TDep1 scopedDependency1,
            TDep2 scopedDependency2) {

            await this.OnInvoke(context, scopedDependency1, scopedDependency2);
            await this.nextRequestDelegate(context);
        }

        protected abstract Task OnInvoke(
            HttpContext context,
            TDep1 scopedDependency1,
            TDep2 scopedDependency2);
    }
}
