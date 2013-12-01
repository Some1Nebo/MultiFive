using System;
using System.Linq;
using System.Security.Principal;
using System.Web.Mvc;
using MultiFive.Domain;
using MultiFive.Web.DataAccess;

namespace MultiFive.Web.Controllers
{
    public class MessageController : Controller
    {
        private readonly Player _player;
        private readonly IPrincipal _user;
        private readonly IRepository _repository;

        public MessageController(IPrincipal user, IRepository repository)
        {
            _user = user;
            _repository = repository;
            _player = _repository.FindPlayer(user);
        }

        public JsonResult Poll(Guid? channelId, int lastMessageId = 0)
        {
            int? pollerId = _user.Identity.IsAuthenticated ? (int?)_player.Id : null;

            var messages = _repository.PollMessages(channelId, pollerId, lastMessageId);

            var jsonMessages = messages.Select(m => new
            {
                id = m.Id,
                name = m.Name,
                content = m.Content
            }).ToArray();

            return Json(jsonMessages, JsonRequestBehavior.AllowGet);
        }
	}
}