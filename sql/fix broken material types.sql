-- get all the affected material type
SELECT [MaterialTypeId]
      ,[Description]
      ,[SortOrder]
  FROM [BioBankingUK].[dbo].[MaterialTypes]
  where [Description] like '%Cerebro%'

-- ensure only one ID actually has associated material details
select count(*), mt.materialtypeid
from MaterialTypes mt
join MaterialDetails md on md.MaterialTypeId = mt.MaterialTypeId
where mt.[Description] like '%Cerebro%'
group by mt.MaterialTypeId

-- delete all the others
SELECT * from MaterialTypes where MaterialTypeId > 51 -- check all latest ones are affected types, not real ones!

 DELETE FROM MaterialTypes WHERE MaterialTypeId > 51