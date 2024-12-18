
namespace Tortoise
{
    internal class AddRoleCommand : TortoiseBotCommand
    {
        public override string GetDisplayName()
        {
            return "Add Role";
        }
        
        public override string GetDescription()
        {
            return "Add a role to a user";
        }
    }
}