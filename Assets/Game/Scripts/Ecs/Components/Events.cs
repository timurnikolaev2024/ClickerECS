namespace Game.Scripts.Ecs.Components
{
    public struct IncomeEvent
    {
        public int BusinessId;
    }

    public struct LevelUpRequest
    {
        public int BusinessId;
    }

    public struct Upgrade1Request : IUpgradeRequest
    {
        public int BusinessId { get; set; }
    }
    
    public struct Upgrade2Request : IUpgradeRequest
    { 
        public int BusinessId { get; set; }
    }
    
    public interface IUpgradeRequest
    {
        int BusinessId { get; set; }
    }
}