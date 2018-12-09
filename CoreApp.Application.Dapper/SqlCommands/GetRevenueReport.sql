alter PROC GetRevenueDaily
	@fromDate VARCHAR(10),
	@toDate VARCHAR(10)
AS
BEGIN
		  select
                CAST(b.DateCreated AS DATE) as Date,
                sum(bd.Quantity*bd.Price) as Revenue,
                sum((bd.Quantity*bd.Price)-(bd.Quantity * p.OriginalPrice)) as Profit
                from Bills b
                inner join dbo.BillDetails bd
                on b.Id = bd.BillId
                inner join Products p
                on bd.ProductId  = p.Id
				where b.DateCreated <= convert(date, @toDate) 
				AND b.DateCreated >= convert(date, @fromDate)
                group by b.DateCreated
END
go

EXEC dbo.GetRevenueDaily @fromDate = '11/15/2018',
                         @toDate = '12/06/2018' 