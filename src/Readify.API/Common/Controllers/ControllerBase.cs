using Microsoft.AspNetCore.Mvc;

namespace Readify.API.Common.Controllers
{
    public abstract class ControllerBase<C>() : Controller where C : ControllerBase
    {

    }
}
