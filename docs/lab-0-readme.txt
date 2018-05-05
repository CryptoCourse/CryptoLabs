1) install iis https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/?view=aspnetcore-2.1&tabs=aspnetcore2x
2) Configure app pool
 * set managed only
 *give folder access for "IIS AppPool\{name}" https://docs.microsoft.com/en-us/iis/manage/configuring-security/application-pool-identities
3) Put files in folder
4) Verify service