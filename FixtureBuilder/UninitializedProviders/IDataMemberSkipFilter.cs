using FixtureBuilder.Helpers;

namespace FixtureBuilder.UninitializedProviders
{
    internal interface IDataMemberSkipFilter
    {
        bool ShouldSkip(DataMemberInfo dataMember, InitializeMembers initializeMembers);
    }
}
