namespace DiscordBot.Configs
{
    public class SalaryConfig : BaseConfig<SalaryConfig>
    {
        public int Sum { get; set; }
        public ChiefsSalary ChiefsSalary { get; set; }
        public int ActionsToSalary { get; set; }  
    }

    public class ChiefsSalary
    {
        public int ChiefModeratorSalary { get; set; }
        public int DeputyChiefModeratorSalary { get; set; }
        public int CuratorSalary { get; set; }
    }
}
