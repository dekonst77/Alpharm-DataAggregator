using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.DataAggregator;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        private DataAggregatorContext _context;

        public NotificationController()
        {
            _context = new DataAggregatorContext(APP);
        }

        ~NotificationController()
        {
            _context.Dispose();
        }

        [HttpGet]
        public ActionResult GetGroupList()
        {
            try
            {
                var result = _context.NotificationGroups.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddGroup(string name)
        {
            try
            {
                if (_context.NotificationGroups.Any(x => x.Name == name))
                    throw new Exception("Группа с таким наименованием уже существует");

                var group = new NotificationGroups()
                {
                    Name = name
                };

                _context.NotificationGroups.Add(group);
                _context.SaveChanges();

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = group
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        

        [HttpPost]
        public ActionResult RenameGroup(int id, string name)
        {
            try
            {
                var group = _context.NotificationGroups.FirstOrDefault(x => x.Id == id);
                if (group != null)
                {
                    using (_context)
                    {
                        group.Name = name;
                        _context.SaveChanges();
                    }
                }

                return new JsonNetResult
                {
                    Data = new JsonResult() { Data = null }
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetGroupUserList(int groupId)
        {
            try
            {
                var result = from g in _context.NotificationGroupUsers.Where(x => x.GroupId == groupId)
                             join u in _context.UserViewAll
                                on g.UserId equals new Guid(u.Id)
                             select new
                             {
                                 g.UserId,
                                 u.FullName,
                                 u.Email
                             };

                return Json(result.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetUserList()
        {
            try
            {
                var result = _context.UserViewAll.ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult AddGroupUsers(int groupId, string[] users)
        {
            try
            {
                if (users != null && users.Length > 0)
                {
                    using (_context)
                    {
                        var exclIds = _context.NotificationGroupUsers
                           .Where(x => x.GroupId == groupId)
                           .Select(x => x.UserId.ToString()).ToArray();
                        var totalUsers = users.Except(exclIds);

                        if (totalUsers.Count() > 0)
                        {
                            _context.NotificationGroupUsers.AddRange(
                                totalUsers.Select(x =>
                                new Domain.Model.DataAggregator.NotificationGroupUsers
                                {
                                    GroupId = groupId,
                                    UserId = new Guid(x)
                                })
                             );
                            _context.SaveChanges();
                        }
                    }
                }

                return new JsonNetResult
                {
                    Data = new JsonResult() { Data = null }
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult RemoveGroupUsers(int groupId, string[] users)
        {
            try
            {
                if (users != null && users.Length > 0)
                {
                    using (_context)
                    {
                        var toDelete = _context.NotificationGroupUsers
                            .Where(x => x.GroupId == groupId && users.Contains(x.UserId.ToString()))
                            .ToArray();

                        if (toDelete != null && toDelete.Any())
                        {
                            _context.NotificationGroupUsers.RemoveRange(toDelete);
                            _context.SaveChanges();
                        }
                    }
                }

                return new JsonNetResult
                {
                    Data = new JsonResult() { Data = null }
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}