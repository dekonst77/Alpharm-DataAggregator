
namespace DataAggregator.Web.Models.LPU
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static DepartmentModel Create(DataAggregator.Domain.Model.LPU.Department model)
        {
            return ModelMapper.Mapper.Map<DepartmentModel>(model);
        }

    }
  /*  public class LPU_DepartmentModel
    {
        public long Id { get; set; }
        public long LPUId_Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public static LPU_DepartmentModel Create(LPU_Department model)
        {
            return ModelMapper.Mapper.Map<LPU_DepartmentModel>(model);
        }
       
    }
*/

}