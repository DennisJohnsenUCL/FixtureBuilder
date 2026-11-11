using FixtureBuilder.Core;

namespace FixtureBuilder.Creation.UninitializedProviders
{
    internal interface IDataMemberSkipFilter
    {
        bool ShouldSkip(DataMemberInfo dataMember, InitializeMembers initializeMembers);
    }
}
