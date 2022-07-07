using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RecaudacionUtils;

namespace RecaudacionApiComprobantePago.Helpers
{
    public class ValidationActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var modelState = context.ModelState;

            if (!modelState.IsValid)
            {
                var response = new StatusResponse<object>();
                response.Messages.Add(new GenericMessage(Definition.MESSAGE_TYPE_ERROR, Message.ERROR_VALIDATION_MODEL));
                response.Success = false;
                context.Result = new OkObjectResult(response);
                //context.Result = new BadRequestObjectResult(modelState);

            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
