using DataAggregator.Domain.DAL;
using DataAggregator.Domain.Model.Retail;
using DataAggregator.Web.Models.Retail;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataAggregator.Web.Controllers.Retail
{
    [Authorize(Roles = "RBoss, RManager")]
    public class SourcePharmaciesEditorController : BaseController
    {
        private readonly RetailContext _context;

        public SourcePharmaciesEditorController()
        {
            _context = new RetailContext(APP);
        }

        ~SourcePharmaciesEditorController()
        {
            _context.Dispose();
        }

        /// <summary>
        /// Возвращает список всех SourcePharmacy
        /// </summary>
        /// <returns>
        /// Список SourcePharmacy в JSON
        /// </returns>
        public ActionResult GetSourcePharmacies()
        {
            var sourcePharmacies = _context.SourcePharmacy.ToList();

            var jsonPharmacies = sourcePharmacies.Select(sourcePharmacy => new SourcePharmacyJson(sourcePharmacy)).ToList();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = jsonPharmacies
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Возвращает список источников
        /// </summary>
        /// <returns>
        /// Список источников в JSON
        /// </returns>
        public ActionResult GetSources()
        {
            var sources = _context.Source.ToList();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = sources
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Возвращает список групп
        /// </summary>
        /// <returns>
        /// Список групп в JSON
        /// </returns>
        public ActionResult GetGroups()
        {
            var groups = _context.SourcePharmacyGroup.ToList();

            var jsonGroups = groups.Select(group => new SourcePharmacyGroupJson(group)).ToList();

            var jsonNetResult = new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = jsonGroups
            };

            return jsonNetResult;
        }

        /// <summary>
        /// Сохраняет группу
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public ActionResult SaveGroup(SourcePharmacyGroupJson dialogData, List<SourcePharmacyGroupFile> filesToSave)
        {
            var groupToSave = _context.SourcePharmacyGroup.Single(g => g.Id == dialogData.Id);
            groupToSave.GroupName = dialogData.GroupName;
            groupToSave.SourceId = dialogData.Source.Id == 0 ? null : (long?)dialogData.Source.Id;

            var filesToAdd = filesToSave.Where(fts => groupToSave.SourcePharmacyGroupFile.All(spgf => fts.Id != spgf.Id)).ToList();

            var filesToRemove = groupToSave.SourcePharmacyGroupFile.Where(spgf => filesToSave.All(fts => fts.Id != spgf.Id)).ToList();

            var filesToChange = filesToSave.Where(fts => groupToSave.SourcePharmacyGroupFile.Any(spgf => spgf.Id == fts.Id)).ToList();

            foreach (var sourcePharmacyGroupFile in filesToAdd)
            {
                groupToSave.SourcePharmacyGroupFile.Add(sourcePharmacyGroupFile);
            }

            foreach (var sourcePharmacyGroupFile in filesToRemove)
            {
                _context.SourcePharmacyGroupFile.Remove(sourcePharmacyGroupFile);
            }

            foreach (var sourcePharmacyGroupFile in filesToChange)
            {
                var fileToChange =
                    groupToSave.SourcePharmacyGroupFile.Single(spgf => spgf.Id == sourcePharmacyGroupFile.Id);
                fileToChange.FileName = sourcePharmacyGroupFile.FileName;
            }

            _context.SaveChanges();
            return null;
        }

        /// <summary>
        /// Сохраняет аптеки
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public ActionResult SavePharmacies(List<SourcePharmacyJson> pharmaciesToSave, List<SourcePharmacyFileJson> filesToSave, FieldsToSaveJson fieldsToSave)
        {
            foreach (SourcePharmacyJson pharmacyJson in pharmaciesToSave)
            {
                SourcePharmacy pharmacyToSave = _context.SourcePharmacy.Single(p => p.Id == pharmacyJson.Id);
                pharmacyToSave.IsSingle = fieldsToSave.IsSingle ? pharmacyJson.IsSingle : pharmacyToSave.IsSingle;
                pharmacyToSave.SourceName = fieldsToSave.SourceName ? pharmacyJson.SourceName : pharmacyToSave.SourceName;
                pharmacyToSave.SourceNameDetailed = fieldsToSave.SourceNameDetailed ? pharmacyJson.SourceNameDetailed : pharmacyToSave.SourceNameDetailed;
                pharmacyToSave.EntityName = fieldsToSave.EntityName ? pharmacyJson.EntityName : pharmacyToSave.EntityName;
                pharmacyToSave.PharmacyName = fieldsToSave.PharmacyName ? pharmacyJson.PharmacyName : pharmacyToSave.PharmacyName;
                pharmacyToSave.PharmacyNumber = fieldsToSave.PharmacyNumber ? pharmacyJson.PharmacyNumber : pharmacyToSave.PharmacyNumber;
                pharmacyToSave.NetName = fieldsToSave.NetName ? pharmacyJson.NetName : pharmacyToSave.NetName;
                pharmacyToSave.Address = fieldsToSave.Address ? pharmacyJson.Address : pharmacyToSave.Address;
                pharmacyToSave.FiasGuid = fieldsToSave.FiasGuid ? pharmacyJson.FiasGuid : pharmacyToSave.FiasGuid;
                //pharmacyToSave.FileName = fieldsToSave.FileName ? pharmacyJson.FileName : pharmacyToSave.FileName;
                //pharmacyToSave.FileName2 = fieldsToSave.FileName2 ? pharmacyJson.FileName2 : pharmacyToSave.FileName2;
                pharmacyToSave.TargetPharmacyId = fieldsToSave.TargetPharmacyId ? pharmacyJson.TargetPharmacyId : pharmacyToSave.TargetPharmacyId;
                pharmacyToSave.SourcePharmacyGroupId = fieldsToSave.SourcePharmacyGroup ? pharmacyJson.SourcePharmacyGroupId : pharmacyToSave.SourcePharmacyGroupId;
                pharmacyToSave.Use = fieldsToSave.Use ? pharmacyJson.Use : pharmacyToSave.Use;

                if (!fieldsToSave.FileNames)
                    continue;

                if (filesToSave == null)
                    filesToSave = new List<SourcePharmacyFileJson>();

                IEnumerable<SourcePharmacyFileJson> filesToAddJson = filesToSave.Where(fts => pharmacyToSave.SourcePharmacyFile.All(spf => fts.Id != spf.Id));

                IList<SourcePharmacyFile> filesToAdd = filesToAddJson.Select(sourcePharmacyFileJson => new SourcePharmacyFile
                {
                    Id = sourcePharmacyFileJson.Id,
                    SourcePharmacyId = sourcePharmacyFileJson.SourcePharmacyId,
                    FileName = sourcePharmacyFileJson.FileName
                })
                    .ToList();

                List<SourcePharmacyFile> filesToRemove = pharmacyToSave.SourcePharmacyFile.Where(spf => filesToSave.All(fts => fts.Id != spf.Id)).ToList();
                List<SourcePharmacyFileJson> filesToChange = filesToSave.Where(fts => pharmacyToSave.SourcePharmacyFile.Any(spf => spf.Id == fts.Id)).ToList();

                foreach (SourcePharmacyFile sourcePharmacyFile in filesToAdd)
                    pharmacyToSave.SourcePharmacyFile.Add(sourcePharmacyFile);

                foreach (SourcePharmacyFile sourcePharmacyFile in filesToRemove)
                    _context.SourcePharmacyFile.Remove(sourcePharmacyFile);

                foreach (SourcePharmacyFileJson sourcePharmacyFileJson in filesToChange)
                {
                    SourcePharmacyFile fileToChange = pharmacyToSave.SourcePharmacyFile.Single(spf => spf.Id == sourcePharmacyFileJson.Id);
                    fileToChange.FileName = sourcePharmacyFileJson.FileName;
                }
            }

            _context.SaveChanges();

            return null;
        }

        /// <summary>
        /// Удаляет группу
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public ActionResult DeleteGroup(long idToDelete)
        {
            var pharmaciesWithGroup = _context.SourcePharmacy.Where(sp => sp.SourcePharmacyGroupId == idToDelete).ToList();

            var groupToDelete = _context.SourcePharmacyGroup.Single(g => g.Id == idToDelete);

            var filesToDelete = _context.SourcePharmacyGroupFile.Where(gf => gf.SourcePharmacyGroupId == idToDelete);

            foreach (var sourcePharmacy in pharmaciesWithGroup)
            {
                sourcePharmacy.SourcePharmacyGroup = null;
            }

            foreach (var sourcePharmacyGroupFile in filesToDelete)
            {
                _context.SourcePharmacyGroupFile.Remove(sourcePharmacyGroupFile);
            }

            _context.SourcePharmacyGroup.Remove(groupToDelete);

            _context.SaveChanges();
            return null;
        }

        /// <summary>
        /// Добавляет группу
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public ActionResult AddGroup(SourcePharmacyGroupJson dialogData, List<SourcePharmacyGroupFile> filesToSave)
        {
            var newGroup = new SourcePharmacyGroup
            {
                GroupName = dialogData.GroupName,
                SourceId = dialogData.Source.Id == 0 ? null : (long?)dialogData.Source.Id
            };

            if (filesToSave != null && filesToSave.Any())
            {
                newGroup.SourcePharmacyGroupFile = new List<SourcePharmacyGroupFile>();
                newGroup.SourcePharmacyGroupFile.AddRange(filesToSave);
            }

            _context.SourcePharmacyGroup.Add(newGroup);
            _context.SaveChanges();

            _context.Entry(newGroup).Reload();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = newGroup
            };
        }

        /// <summary>
        /// Добавляет аптеку
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public ActionResult AddPharmacy(SourcePharmacyJson pharmacyToAdd, List<SourcePharmacyFileJson> filesToAdd)
        {
            var newPharmacy = new SourcePharmacy
            {
                IsSingle = pharmacyToAdd.IsSingle,
                SourceName = pharmacyToAdd.SourceName,
                SourceNameDetailed = pharmacyToAdd.SourceNameDetailed,
                EntityName = pharmacyToAdd.EntityName,
                PharmacyName = pharmacyToAdd.PharmacyName,
                PharmacyNumber = pharmacyToAdd.PharmacyNumber,
                NetName = pharmacyToAdd.NetName,
                Address = pharmacyToAdd.Address,
                FiasGuid = pharmacyToAdd.FiasGuid,
                //FileName = pharmacyToAdd.FileName,
                //FileName2 = pharmacyToAdd.FileName2,
                TargetPharmacyId = pharmacyToAdd.TargetPharmacyId,
                SourcePharmacyGroupId = pharmacyToAdd.SourcePharmacyGroupId,
                Use = pharmacyToAdd.Use
            };

            if (filesToAdd != null && filesToAdd.Any())
            {
                newPharmacy.SourcePharmacyFile = new List<SourcePharmacyFile>();
                foreach (var sourcePharmacyFileJson in filesToAdd)
                {
                    newPharmacy.SourcePharmacyFile.Add(new SourcePharmacyFile
                    {
                        FileName = sourcePharmacyFileJson.FileName
                    });
                }
            }

            _context.SourcePharmacy.Add(newPharmacy);
            _context.SaveChanges();

            _context.Entry(newPharmacy).Reload();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = newPharmacy
            };
        }

        /// <summary>
        /// Удаляет аптеки
        /// </summary>
        /// <returns>
        /// </returns>
        [HttpPost]
        public ActionResult DeletePharmacies(List<long> idsToDelete)
        {
            var pharmaciesToDelete = _context.SourcePharmacy.Where(p => idsToDelete.Contains(p.Id));

            var filesToDelete = _context.SourcePharmacyFile.Where(f => idsToDelete.Contains(f.SourcePharmacyId));

            foreach (var sourcePharmacyFile in filesToDelete)
            {
                _context.SourcePharmacyFile.Remove(sourcePharmacyFile);
            }

            _context.Database.ExecuteSqlCommand(string.Format("delete from SourcePharmacyRelevance where SourcePharmacyId in ({0})", string.Join(", ", idsToDelete)));

            foreach (var sourcePharmacy in pharmaciesToDelete)
            {
                _context.SourcePharmacy.Remove(sourcePharmacy);
            }

            _context.SaveChanges();

            return null;
        }


        /// <summary>
        /// Возвращает аптеку со списком файлов, принадлежащих ей
        /// </summary>
        /// <returns>
        /// Объект аптеки
        /// </returns>
        [HttpPost]
        public ActionResult GetPharmacy(long pharmacyId)
        {
            var pharmacy = _context.SourcePharmacy.Single(sp => sp.Id == pharmacyId);

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = pharmacy
            };
        }

        /// <summary>
        /// Возвращает группу со списком файлов, принадлежащих ей
        /// </summary>
        /// <returns>
        /// Объект группы
        /// </returns>
        [HttpPost]
        public ActionResult GetGroup(long groupId)
        {
            var group = _context.SourcePharmacyGroup.Single(g => g.Id == groupId);

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = group
            };
        }

        /// <summary>
        /// Возвращает аптеку со списком файлов, принадлежащих ей
        /// </summary>
        /// <returns>
        /// Объект объединенной аптеки
        /// </returns>
        [HttpPost]
        public ActionResult MergePharmacies(List<SourcePharmacyJson> pharmaciesToMerge, long mergeTo)
        {
            var mergeToPharmacy = _context.SourcePharmacy.Single(sp => sp.Id == mergeTo);

            var mergeFromIds = pharmaciesToMerge.Where(ptm => ptm.Id != mergeTo).Select(ptm => ptm.Id).ToList();
            var mergeFromPharmacies = _context.SourcePharmacy.Where(sp => mergeFromIds.Contains(sp.Id)).ToList();

            foreach (var mergeFromPharmacy in mergeFromPharmacies)
            {
                foreach (var sourcePharmacyFile in mergeFromPharmacy.SourcePharmacyFile)
                {
                    sourcePharmacyFile.SourcePharmacy = mergeToPharmacy;
                }
            }

            _context.Database.ExecuteSqlCommand(string.Format("delete from SourcePharmacyRelevance where SourcePharmacyId in ({0})", string.Join(", ", mergeFromIds)));
            _context.Database.ExecuteSqlCommand(string.Format("update RawData set SourcePharmacyId = {0} where SourcePharmacyId in ({1})", mergeToPharmacy.Id, string.Join(", ", mergeFromIds)));

            _context.SourcePharmacy.RemoveRange(mergeFromPharmacies);

            _context.SaveChanges();
            _context.Entry(mergeToPharmacy).Reload();

            return new JsonNetResult
            {
                Formatting = Formatting.Indented,
                Data = mergeToPharmacy
            };
        }

        [HttpPost]
        public ActionResult ImportPharmacies_from_Excel(IEnumerable<HttpPostedFileBase> uploads)
        {
            try
            {
                if (uploads == null || !uploads.Any())
                    return null;

                var file = uploads.First();
                string filename = @"\\s-sql2\Upload\NewPharmacies_" + User.Identity.GetUserId() + ".xlsx";

                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);

                file.SaveAs(filename);

                _context.ImportPharmacies_from_Excel(filename);

                return new JsonNetResult
                {
                    Formatting = Formatting.Indented,
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}