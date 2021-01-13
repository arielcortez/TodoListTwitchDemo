using System.Collections.Generic;
using System.Web.Http;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Todo.AzureFunctions.Constants;
using Todo.AzureFunctions.Entities;
using Todo.AzureFunctions.Factories;
using Todo.AzureFunctions.Factories.Factories;
using Todo.AzureFunctions.Services.Interfaces;
using Todo.Shared.Constants;
using Todo.Shared.Dto.TodoItems;

namespace Todo.AzureFunctions.Functions.TodoItems
{
    public class GetItemsOfTodoListFunction
    {
        private readonly CloudTable _cloudTable;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public GetItemsOfTodoListFunction(ICloudTableFactory cloudTableFactory, IMapper mapper, IAuthService authService)
        {
            _mapper = mapper;
            _authService = authService;
            _cloudTable = cloudTableFactory.CreateCloudTable(TableStorageConstants.TodoItemTable);
        }


        [FunctionName(FunctionConstants.GetItemsOfTodoListFunction)]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
            HttpRequest req)
        {
            var user = _authService.GetClientPrincipalFromRequest(req);
            var listId = user.UserId;


            if (string.IsNullOrEmpty(listId))
            {
                return new BadRequestErrorMessageResult("Id cannot be empty");
            }

            var query = new TableQuery<TodoItemEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, listId));

            var todoList = _cloudTable.ExecuteQuery(query);

            return new OkObjectResult(_mapper.Map<IEnumerable<TodoItemDto>>(todoList));
        }
    }
}