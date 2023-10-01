using Anndotnet.App.Models;

namespace Anndotnet.App.Messages;
public record class RunExampleMessage(string name);
public record class CreatePageMessage(NavigationItem navItem);

