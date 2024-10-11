using Module_19_Final.BLL.Exceptions;
using Module_19_Final.BLL.Models;
using Module_19_Final.BLL.Services;
using Module_19_Final.DAL.Entities;
using Module_19_Final.DAL.Repositories;
using Moq;

namespace Module19Final.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void AddFriend_UserAndFriendExist_AddsFriendSuccessfully()
        {
            // Arrange 
            var userRepositoryMock = new Mock<IUserRepository>();
            var friendRepositoryMock = new Mock<IFriendRepository>();

            var user = new User(
                id: 1,
                firstName: "Максим",
                lastName: "Иванов",
                password: "1112212",
                email: "user@example.com",
                photo: "jpg",
                favoriteMovie: "Бэтмен",
                favoriteBook: "Гарри Поттер",
                incomingMessages: new List<Message>(),
                outgoingMessages: new List<Message>(),
                friends: new List<User>()
            );

            var friend = new User(
                id: 2,
                firstName: "Алексей",
                lastName: "Петров",
                password: "123456",
                email: "friend@example.com",
                photo: "png",
                favoriteMovie: "Супермен",
                favoriteBook: "Властелин Колец",
                incomingMessages: new List<Message>(),
                outgoingMessages: new List<Message>(),
                friends: new List<User>()
            );

            var userEntity = MapToUserEntity(user);
            var friendEntity = MapToUserEntity(friend);

            userRepositoryMock.Setup(repo => repo.FindByEmail("user@example.com"))
                .Returns(userEntity);
            userRepositoryMock.Setup(repo => repo.FindByEmail("friend@example.com"))
                .Returns(friendEntity);

            friendRepositoryMock.Setup(repo => repo.Create(It.IsAny<FriendEntity>())).Returns(1);

            var userService = new UserService(userRepositoryMock.Object, friendRepositoryMock.Object);

            var userAddingFriendData = new UserAddingFriendData
            {
                UserId = user.Id,
                FriendEmail = "friend@example.com"
            };

            // Act 
            userService.AddFriend(userAddingFriendData);

            // Assert 
            friendRepositoryMock.Verify(repo => repo.Create(It.Is<FriendEntity>(
                f => f.user_id == userEntity.id && f.friend_id == friendEntity.id)), Times.Once);
        }

        [Fact]
        public void AddFriend_UserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange 
            var userRepositoryMock = new Mock<IUserRepository>();
            var friendRepositoryMock = new Mock<IFriendRepository>();

            userRepositoryMock.Setup(repo => repo.FindByEmail("user@example.com"))
                .Returns((UserEntity)null);

            var userService = new UserService(userRepositoryMock.Object, friendRepositoryMock.Object);

            var userAddingFriendData = new UserAddingFriendData
            {
                UserId = 1,
                FriendEmail = "friend@example.com"
            };

            // Act  и Assert 
            Assert.Throws<UserNotFoundException>(() =>
                userService.AddFriend(userAddingFriendData));
        }

        [Fact]
        public void AddFriend_FriendNotFound_ThrowsUserNotFoundException()
        {
            // Arrange 
            var userRepositoryMock = new Mock<IUserRepository>();
            var friendRepositoryMock = new Mock<IFriendRepository>();

            var user = new User(
                id: 1,
                firstName: "Джон",
                lastName: "Доу",
                password: "password",
                email: "john.doe@example.com",
                photo: "path/to/photo",
                favoriteMovie: "Начало",
                favoriteBook: "1984",
                incomingMessages: new List<Message>(),
                outgoingMessages: new List<Message>(),
                friends: new List<User>()
            );

            userRepositoryMock.Setup(repo => repo.FindByEmail("user@example.com"))
                .Returns(MapToUserEntity(user));
            userRepositoryMock.Setup(repo => repo.FindByEmail("friend@example.com"))
                .Returns((UserEntity)null);

            var userService = new UserService(userRepositoryMock.Object, friendRepositoryMock.Object);

            var userAddingFriendData = new UserAddingFriendData
            {
                UserId = 1,
                FriendEmail = "friend@example.com"
            };

            // Act  и Assert 
            Assert.Throws<UserNotFoundException>(() =>
                userService.AddFriend(userAddingFriendData));
        }

        private UserEntity MapToUserEntity(User user)
        {
            return new UserEntity
            {
                id = user.Id,
                firstname = user.FirstName,
                lastname = user.LastName,
                password = user.Password,
                email = user.Email,
                photo = user.Photo,
                favorite_movie = user.FavoriteMovie,
                favorite_book = user.FavoriteBook
            };
        }
    }
}
