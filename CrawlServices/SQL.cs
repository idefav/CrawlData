using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlServices
{
    public class SQL
    {
        public const string Db_IndexIsExist = @"select count(*) from sys.indexes where name='{0}';";

        /// <summary>
        /// 创建抓取数据索引
        /// </summary>
        public const string DB_Taobao_data_createindex_updatetime =
            @"CREATE NONCLUSTERED INDEX [{0}] ON [dbo].[{1}] 
(
	[UpdateTime] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]";


        public const string DB_CreateTable_Sql = @"CREATE TABLE [dbo].[{0}](

                [Guid][nvarchar](50) NOT NULL,

                [ItemId][nvarchar](50) NOT NULL,

                [Title][nvarchar](max) NULL,

                [Price][decimal](18, 2) NULL,

                [PriceAVG][decimal](18, 2) NULL,

                [SellCount][int] NULL,

                [PriceUnit][nvarchar](50) NULL,

                [CountUnit][nvarchar](50) NULL,

                [CountAVG][int] NULL,

                [PDate][datetime] NOT NULL,

                [UpdateTime][datetime] NOT NULL,

                [CommentCount][int] NULL,

                [PicUrl][nvarchar](max) NULL,
                CONSTRAINT[PK_{0}] PRIMARY KEY CLUSTERED
            (

                [ItemId] ASC,

                [PDate] ASC
            )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
            ) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]";
    }
}
