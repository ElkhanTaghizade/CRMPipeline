namespace CRM_Pipeline.Models
{
    public class SwapStagesDTO
    {
        public Guid DraggedStageId { get; set; }
        public Guid TargetStageId { get; set; }
    }
}
