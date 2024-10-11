using Module_19_Final.DAL.Entities;

namespace Module_19_Final.DAL.Repositories
{
    public interface IFriendRepository
    {
        int Create(FriendEntity friendEntity);
        IEnumerable<FriendEntity> FindAllByUserId(int userId);
        int Delete(int id);
    }
}
