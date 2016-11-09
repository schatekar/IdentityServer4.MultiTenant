namespace product1.MultiTenancy
{
    public class CollinsonTenant
    {
        public string Name { get; set; }
        public IdentityConfig Identity { get; set; }
        public string Hostname { get; set; }
    }
}