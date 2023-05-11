using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskSystem.Commands.Task;
using TaskSystem.DTO.Task.Queries;
using TaskSystem.DTO.Task.Inputs;
using TaskSystem.Enums;
using TaskSystem.DTO.Task;
using TaskSystem.Filters;
using TaskSystem.Models;

namespace TaskSystem.Controllers
{
    [Route("api/v1/tasks")]
    [ApiController]
    [ServiceFilter(typeof(UserAuthorizationFilter))]
    public class TaskController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ListTaskItemDto>> List([FromQuery] ListTaskItemsQuery query)
        {
            var command = new ListTaskItemsCommand
            (
                state: query.State
            );

            return Ok(await _mediator.Send(command));
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItemInput input)
        {
            var command = new CreateTaskItemCommand
            (
                name: input.Name,
                description: input.Description
            );

            return Created(nameof(Get), await _mediator.Send(command));
        }

        [HttpGet(template: "{id}")]
        public async Task<ActionResult<TaskItem>> Get(string id)
        {
            var command = new GetTaskItemCommand
            (
                id: id
            );

            return Ok(await _mediator.Send(command));
        }

        [HttpPost(template: "{id}/comments")]
        public async Task<ActionResult> AddComment(string id, [FromBody] CommentItemInput content)
        {
            var command = new AddCommentToTaskItemCommand
            (
                id: id,
                content: content.Content
            );

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPost(template: "{id}/approve")]
        public async Task<ActionResult<TaskItem>> Approve(string id, [FromBody] CommentItemInput? comment)
        {
            var request = new ChangeTaskStateCommand
            (
                id: id,
                state: TaskStateEnum.Resolved,
                comment: comment?.Content.Length > 0 ? comment.Content : null
            );

            return Ok(await _mediator.Send(request));
        }

        [HttpPost(template: "{id}/reject")]
        public async Task<ActionResult<TaskItem>> Reject(string id, [FromBody] CommentItemInput comment)
        {
            var request = new ChangeTaskStateCommand
            (
                id: id,
                state: TaskStateEnum.Rejected,
                comment: comment.Content
            );

            return Ok(await _mediator.Send(request));
        }

        [HttpPost(template: "{id}/reopen")]
        public async Task<ActionResult<TaskItem>> Reopen(string id, [FromBody] CommentItemInput comment)
        {
            var request = new ChangeTaskStateCommand
            (
                id: id,
                state: TaskStateEnum.Reopened,
                comment: comment.Content
            );

            return Ok(await _mediator.Send(request));
        }
    }
}