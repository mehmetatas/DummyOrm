# DummyOrm

DummyOrm is the ORM tool that is designed for fast development. The main idea is:

> Do general stuff with the ORM but for complex cases like reporting queries (group by, having, too many joins etc.) write inline sql queries or use stored procedures.

So, DummyOrm does not support many features that are supported by popular ORMs

 - It is not an UnitOfwork/Repository implementation (ISession/DbContext)
 - No LINQ Provider
 - No lazy loading
 - No implicit OneToMany support
 - No first and second level caching
 - No cascading
 - Only supports SqlServer 2014
 - No code first support
 - No GROUP BY support
 - No unit tests!!! (Yes, it is not tested! yet...)

Then, what does it support?

First things you need to know.

    One Entity      =   One Table
    Class Name      =   Table Name
    Property Name   =   Column Name

Let's look at the `User` entity and table.

    public class User {                                  CREATE TABLE [User] (
        public long Id { get; set; }                       [Id] [bigint] IDENTITY(1,1) NOT NULL,
        public string Fullname { get; set; }               [Fullname] [nvarchar](100) NULL,
        public string Email { get; set; }                  [Email] [nvarchar](100) NULL,
        public string Username { get; set; }               [Username] [nvarchar](20) NULL,
        public string Password { get; set; }               [Password] [nvarchar](32) NULL,
        public DateTime JoinDate { get; set; }             [JoinDate] [datetime] NULL,
        public UserType Type { get; set; }                 [Type] [int] NULL,
        public UserStatus Status { get; set; }             [Status] [int] NULL,
    }                                                      PRIMARY KEY CLUSTERED ([Id] ASC)
                                                         ) ON [PRIMARY]

Entities should be POCO classes with a public parameterless constuctor and public properties with both get and set. Since DummyOrm does not have a code first feature it is your responsibility to match the table/column names and types.

Here is the default CRL/Db type mapping used. For value types, their `Nullable<>` versions are also supported.

    { typeof(byte),       DbType.Byte },
    { typeof(sbyte),      DbType.SByte },
    { typeof(short),      DbType.Int16 },
    { typeof(ushort),     DbType.UInt16 },
    { typeof(int),        DbType.Int32 },
    { typeof(uint),       DbType.UInt32 },
    { typeof(long),       DbType.Int64 },
    { typeof(ulong),      DbType.UInt64 },
    { typeof(float),      DbType.Single },
    { typeof(double),     DbType.Double },
    { typeof(decimal),    DbType.Decimal },
    { typeof(bool),       DbType.Boolean },
    { typeof(char),       DbType.StringFixedLength },
    { typeof(string),     DbType.String },
    { typeof(byte[]),     DbType.Binary },
    { typeof(Guid),       DbType.Guid },
    { typeof(DateTime),   DbType.DateTime2 }

> Enums (and nullable enums) are supported too!

Now introduce this entity to our DummyOrm.

    DbMeta.Instance.RegisterEntity<User>();

After this registration DummyOrm assumes there exists a User table in the database, with columns according to conventions described above. If there is a property named `Id` it is assumed as the autoincrement primary key.

### Single Table Operations

Let's create an `IDb` instance which we will be using for database operations.

    SqlConnection conn = ...
    using (IDb db = new DbImpl(conn))
    {
        
    } // Will dispose the connection
    
### Insert

**C&num;**

    User user = new User {  Username = "mehmetatas" };
    db.Insert(user);
    Console.WriteLine(user.Id);

**SQL**
    
    INSERT INTO [User] ([Fullname],[Email],[Username],[Password],[JoinDate],[Type],[Status])
    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6); SELECT SCOPE_IDENTITY();
    -- @p2 = 'mehmetatas', other parameters are null or at default
    
> Autoincrement primary key value (Id property) will be set after insert.
    
### Update

**C&num;**

    User user = new User {  Id = 42, Username = "mehmet" };
    db.Update(user);
 
**SQL**
    
    UPDATE [User] SET [Fullname]=@p0,[Email]=@p1,[Username]=@p2,[Password]=@p3,[JoinDate]=@p4,[Type]=@p5,[Status]=@p6 WHERE [Id]=@wp7
    -- @wp7 = 42, @p2 = 'mehmet', other parameters are null or at default

### Delete

**C&num;**

    User user = new User {  Id = 42 };
    db.Delete(user);

**SQL**
        
    DELETE FROM [User] WHERE [Id]=@wp0
    -- @wp0 = 42
    
> For update and delete operations entity's primary key value must be set.

### GetById

**C&num;**

    User user = db.GetById<User>(42);

**SQL**
        
    SELECT [Id],[Fullname],[Email],[Username],[Password],[JoinDate],[Type],[Status] FROM [User] WHERE [Id]=@wp0
    -- @wp0 = 42
    
### Select - List All Records

**C&num;**

    IList<User> list = db.Select<User>()
                         .ToList();

**SQL**
    
    SELECT
      u.Id u_Id,
      u.Fullname u_Fullname,
      u.Email u_Email,
      u.Username u_Username,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status
    FROM [User] u

### Select - Order By

**C&num;**

    IList<User> list = db.Select<User>()
                         .OrderBy(u => u.Fullname)
                         .OrderByDesc(u => u.Username)
                         .ToList();

**SQL**
    
    SELECT
      u.Fullname u_Fullname,
      u.Username u_Username,
      u.Id u_Id,
      u.Email u_Email,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status
    FROM [User] u
    ORDER BY u.Fullname ASC,u.Username DESC

### Select - Simple Where

**C&num;**

    User user = db.Select<User>()
                  .Where(u => u.Id == 42)
                  .FirstOrDefault();

**SQL**
    
    SELECT TOP 1
      u.Id u_Id,
      u.Fullname u_Fullname,
      u.Email u_Email,
      u.Username u_Username,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status
    FROM [User] u
    WHERE
    u.Id = @wp0
    -- @wp0 = 42
    
### Select - Complex Where

**C&num;**

    var userIds = new[] { 40L, 41L, 43L, 44L };
    IList<User> list = db.Select<User>()
                         .Where(u => userIds.Contains(u.Id) || u.Username.Contains("us"))
                         .Where(u => !u.Email.EndsWith("@hotmail.com"))
                         .ToList();
 
**SQL**
                            
    SELECT
      u.Id u_Id,
      u.Fullname u_Fullname,
      u.Email u_Email,
      u.Username u_Username,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status
    FROM [User] u
    WHERE
    ((u.Id IN (@wp0,@wp1,@wp2,@wp3) OR u.Username LIKE @wp4) AND NOT (u.Email LIKE @wp5))
    -- @wp0=40, @wp1=41, @wp2=43, @wp3=44, @wp4=%us%, @wp5=%@hotmail.com

> DummyOrm does not have a LINQ provider but can convert some basic LINQ expressions to SQL. It is higly possible to get some exception if you try some complex things inside `Where`.

### Select - Where Support

    &&                            ->    AND
    ||                            ->    OR
    !                             ->    NOT
    user.Id == 42                 ->    u.Id = 42
    user.Id != 42                 ->    u.Id <> 42
    user.Id < 42                  ->    u.Id < 42
    user.Id <= 42                 ->    u.Id <= 42
    user.Id > 42                  ->    u.Id > 42
    user.Id >= 42                 ->    u.Id >= 42
    String.Contains("abc")        ->    LIKE %abc%
    String.StartsWith("abc")      ->    LIKE abc%
    String.EndsWith("abc")        ->    LIKE %abc
    Enumerable.Contains(user.Id)  ->    IN (...)
    List<>.Contains(user.Id)      ->    IN (...)
    
> For some cases these operations might not work too. (Remember DummyOrm is not tested! yet...)

### Select - Paging

**C&num;**

    Page<User> page = db.Select<User>()
                        .Page(3, 10); // Page: 3, PageSize: 10
    
    User[] users = page.Items;          // 10 Users
    
    Console.WriteLine(page.PageIndex);  // 3
    Console.WriteLine(page.PageSize);   // 10
    Console.WriteLine(page.PageCount);  // 5
    Console.WriteLine(page.TotalCount); // 42
    Console.WriteLine(page.HasMore);    // True (page.PageIndex < page.PageCount)

**SQL**

    WITH __DATA AS (
      SELECT
        u.Id u_Id,
        u.Fullname u_Fullname,
        u.Email u_Email,
        u.Username u_Username,
        u.Password u_Password,
        u.JoinDate u_JoinDate,
        u.Type u_Type,
        u.Status u_Status
      FROM [User] u
    ),
    __COUNT AS (SELECT COUNT(0) AS __ROWCOUNT FROM __DATA)
    SELECT * FROM __COUNT, __DATA
    ORDER BY __DATA.u_Id
    OFFSET 20 ROWS FETCH NEXT 10 ROWS ONLY
    
> See: http://stackoverflow.com/a/20130331/554397

### Select - Top

**C&num;**

    Page<User> page = db.Select<User>()
                        .Top(10);
                        
    User[] users = page.Items;          // 10 Users
    
    Console.WriteLine(page.PageIndex);  // 1
    Console.WriteLine(page.PageSize);   // 10
    Console.WriteLine(page.PageCount);  // 2
    Console.WriteLine(page.TotalCount); // 11
    Console.WriteLine(page.HasMore);    // True (page.PageIndex < page.PageCount)
                        
**SQL**

    SELECT TOP 11
      u.Id u_Id,
      u.Fullname u_Fullname,
      u.Email u_Email,
      u.Username u_Username,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status
    FROM [User] u

> Top actually selects (top + 1) records to see if there are more records. But it returns only (top) records.

### Select - Include Single Column

**C&num;**

    IList<User> list = db.Select<User>()
                         .Include(u => u.Username)
                         .ToList();

**SQL**

    SELECT
      u.Username u_Username,
      u.Id u_Id
    FROM [User] u
  
> Primary key will be selected even if it is not specified.
  
### Select - Include Multiple Columns

**C&num;**

    IList<User> list = db.Select<User>()
                         .Include(u => new { u.Fullname, u.Username, u.Email })
                         .ToList();

**SQL**
  
    SELECT
      u.Fullname u_Fullname,
      u.Username u_Username,
      u.Email u_Email,
      u.Id u_Id
    FROM [User] u
    
## Relations
    
Till here, all queries are executed against a single, the [User] table. In other words, there were no `JOIN`s. First define a new entity that is associatied with the `User` entity then we will see some `JOIN`s.

    public class Post {                                  CREATE TABLE [Post2] (
        public long Id { get; set; }                      [Id] [bigint] IDENTITY(1,1) NOT NULL,
        public User User { get; set; }                    [UserId] [bigint] NULL,
        public DateTime CreateDate { get; set; }          [CreateDate] [datetime] NULL,
        public DateTime? PublishDate { get; set; }        [PublishDate] [datetime] NULL,
        public DateTime? UpdateDate { get; set; }         [UpdateDate] [datetime] NULL,
        public string Title { get; set; }                 [Title] [nvarchar](255) NULL,
        public string HtmlContent { get; set; }           [HtmlContent] [nvarchar](255) NULL,
        public AccessLevel AccessLevel { get; set; }      [AccessLevel] [int] NULL,
    }                                                     PRIMARY KEY CLUSTERED ([Id] ASC)
                                                         ON [PRIMARY])

### Insert - Entity With Relation

**C&num;**

    var post = new Post
    {
        CreateDate = DateTime.UtcNow,
        Title = "My First Post",
        HtmlContent = "<p>Lorem Ipsum...</p>",
        User = new User
        {
            Id = 42
        }
    };
    
    db.Insert(post);
    
    Console.WriteLine(post.Id);

**SQL**
  
    INSERT INTO [Post] ([UserId],[CreateDate],[PublishDate],[UpdateDate],[Title],[HtmlContent],[AccessLevel],[Data])
    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7); SELECT SCOPE_IDENTITY();
    -- @p0=42, other parameters are as expected
    
### Insert - Cascade

**C&num;**

    var post = new Post
    {
        CreateDate = DateTime.UtcNow,
        Title = "My First Post",
        HtmlContent = "<p>Lorem Ipsum...</p>",
        User = new User
        {
            Username = "mehmet"
        }
    };
    
    db.Insert(post.User);
    db.Insert(post);
    
    Console.WriteLine(post.Id);
    
**SQL**
 
    INSERT INTO [User] ([Fullname],[Email],[Username],[Password],[JoinDate],[Type],[Status]) 
    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6); SELECT SCOPE_IDENTITY();
    
    INSERT INTO [Post] ([UserId],[CreateDate],[PublishDate],[UpdateDate],[Title],[HtmlContent],[AccessLevel],[Data])
    VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7); SELECT SCOPE_IDENTITY();

> Cascade operations are not supported too. If you want cascade behavior, you simply do it yourself.

### Select
 
**C&num;**

    IList<Post> list = db.Select<Post>()
                         .ToList();

    foreach (var post in list)
    {
        Console.WriteLine(post.User.Id > 0);           // True
        Console.WriteLine(post.User.Username == null); // True
    }
    
**SQL**
 
    SELECT
      p.Id p_Id,
      p.UserId p_UserId,
      p.CreateDate p_CreateDate,
      p.PublishDate p_PublishDate,
      p.UpdateDate p_UpdateDate,
      p.Title p_Title,
      p.HtmlContent p_HtmlContent,
      p.AccessLevel p_AccessLevel,
      p.Data p_Data
    FROM [Post] p
    
 > Note that if `UserId` column of `Post` table is `NULL` then `User` property of the `Post` class will be null too.
 
### Select - Join Related Entity
  
**C&num;**

    IList<Post> list = db.Select<Post>()
                         .Join(p => p.User)
                         .ToList();

    foreach (var post in list)
    {
        Console.WriteLine(post.User.Id > 0);           // True
        Console.WriteLine(post.User.Username == null); // False
    }
                         
**SQL**
 
     SELECT
      u.Id u_Id,
      p.UserId p_UserId,
      u.Fullname u_Fullname,
      u.Email u_Email,
      u.Username u_Username,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status,
      p.Id p_Id,
      p.CreateDate p_CreateDate,
      p.PublishDate p_PublishDate,
      p.UpdateDate p_UpdateDate,
      p.Title p_Title,
      p.HtmlContent p_HtmlContent,
      p.AccessLevel p_AccessLevel,
      p.Data p_Data
    FROM [Post] p
      INNER JOIN [User] u ON p.UserId = u.Id
      
### Select - Specific Columns

**C&num;**

    IList<Post> list = db.Select<Post>()
                         .Join(p => p.User, u => new { u.Username, u.Fullname })
                         .Include(p => new { p.Title, p.PublishDate })
                         .ToList();
                    
**SQL**
 
    SELECT
      u.Id u_Id,
      u.Username u_Username,
      u.Fullname u_Fullname,
      p.Title p_Title,
      p.PublishDate p_PublishDate,
      p.Id p_Id
    FROM [Post] p
      INNER JOIN [User] u ON p.UserId = u.Id
      
### Include vs Join

**Include**, is used for indicating which columns we want to select.

**Join**, is used for indicating that we want to select the related entity with a single query. Second parameter of the Join method behaves same way as the Include method this time we specify the columns of related entity.

Some quick samples

    // Select all columns of Post
    // Join User and select all columns of User
    .Join(p => p.User) 
    
    // Select all columns of Post
    // Join User and only select username of User. (Id will be selected even it is not specified explicitly)
    .Join(p => p.User, u => new { u.Username })
    
    // Select only Title and PublishDate of the post
    // Join User and just select its Username
    .Include(p => new p { p.Title, p.PublishDate })
    .Join(p => p.User, u => new { u.Username })
    
    // Only select User of the Post (No Title, no PublishDate, no HtmlContent etc.)
    // And select everything about the user. This will implicitly cause a Join
    .Include(p => p.User)
    
### Select - Implicit Join Caused By Include

**C&num;**

    IList<Post> list = db.Select<Post>()
                         .Include(p => p.User)
                         .ToList();
                
**SQL**
 
    SELECT
      p.UserId p_UserId,
      u.Id u_Id,
      u.Fullname u_Fullname,
      u.Email u_Email,
      u.Username u_Username,
      u.Password u_Password,
      u.JoinDate u_JoinDate,
      u.Type u_Type,
      u.Status u_Status,
      p.Id p_Id
    FROM [Post] p
      INNER JOIN [User] u ON p.UserId = u.Id
      
### Select - Implicit Join Caused By Where

**C&num;**

    IList<Post> list = db.Select<Post>()
                         .Where(p => p.User.Username.Contains("mehmet"))
                         .ToList();
                
**SQL**
 
    SELECT
      u.Id u_Id,
      p.Id p_Id,
      p.UserId p_UserId,
      p.CreateDate p_CreateDate,
      p.PublishDate p_PublishDate,
      p.UpdateDate p_UpdateDate,
      p.Title p_Title,
      p.HtmlContent p_HtmlContent,
      p.AccessLevel p_AccessLevel,
      p.Data p_Data
    FROM [Post] p
      INNER JOIN [User] u ON p.UserId = u.Id
    WHERE
    u.Username LIKE @wp0
    
### Select - Implicit Join Caused By OrderBy

**C&num;**

    IList<Post> list = db.Select<Post>()
                         .OrderBy(p => p.User.Username)
                         .ToList();
                
**SQL**
 
    SELECT
      u.Id u_Id,
      u.Username u_Username,
      p.Id p_Id,
      p.UserId p_UserId,
      p.CreateDate p_CreateDate,
      p.PublishDate p_PublishDate,
      p.UpdateDate p_UpdateDate,
      p.Title p_Title,
      p.HtmlContent p_HtmlContent,
      p.AccessLevel p_AccessLevel,
      p.Data p_Data
    FROM [Post] p
      INNER JOIN [User] u ON p.UserId = u.Id
    ORDER BY u.Username ASC
    
# TODO
    
## Association Entities

Association entities are the entities that are used for building ManyToMany associations between two entities. Association entities do not have a autoincrement primary key. They use the foreign keys as the composite primary key.

First introduce our new entity `Like`.

    public class Like {                                 CREATE TABLE [Like] (
        public Post Post { get; set; }                      [PostId] [bigint] NOT NULL,
        public User User { get; set; }                      [UserId] [bigint] NOT NULL,
        public DateTime LikedDate { get; set; }             [LikedDate] [datetime] NULL,
    }                                                   	CONSTRAINT [PK_Like] PRIMARY KEY CLUSTERED 
                                                        	(
                                                        		[PostId] ASC,
                                                        		[UserId] ASC
                                                        	)
                                                        ) ON [PRIMARY]                

Register the association entity.

    DbMeta.Instance.RegisterEntity<Like>();
    
If a registered entity does not have a property named `Id` then it is assumed to be an Association entity. An Association entity must have exactly two properties those are referencing to another entities. Other primitive typed properties can also be added on to association entities.

## Collection Properties

## OneToMany

## ManyToMany

## Views

## Inline Sql

## Stored Procedure