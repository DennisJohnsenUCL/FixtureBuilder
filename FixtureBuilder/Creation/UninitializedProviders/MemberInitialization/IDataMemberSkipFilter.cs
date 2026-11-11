using FixtureBuilder.Core;

namespace FixtureBuilder.Creation.UninitializedProviders.MemberInitialization
{
    internal interface IDataMemberSkipFilter
    {
        bool ShouldSkip(DataMemberInfo dataMember, InitializeMembers initializeMembers);
    }
}
