using Carbunql;
using Carbunql.TypeSafe;
using Carbunql.Building;

internal class Program
{
    private static void Main(string[] args)
    {
        var staffId = 1;

        //全件取得
        Console.WriteLine(SelectAreaSaleReport().ToText());
        Console.WriteLine(";");

        Console.WriteLine(SelectAreaSaleReportByStaffId(staffId).ToText());
        Console.WriteLine(";");

        //帳票別フィルタリング
        Console.WriteLine(SelectStoreSaleReport().ToText());
        Console.WriteLine(";");

        Console.WriteLine(SelectStoreSaleReportByStaffId(staffId).ToText());
        Console.WriteLine(";");

        //拡張メソッドによるフィルタリング
        Console.WriteLine(SelectAreaSaleReport().FilterByStaffId(staffId).ToText());
        Console.WriteLine(";");

        Console.WriteLine(SelectStoreSaleReport().FilterByStaffId(staffId).ToText());
        Console.WriteLine(";");
    }

    /// <summary>
    /// Area_sale_reportを全件取得します
    /// </summary>
    /// <returns></returns>
    private static FluentSelectQuery<Area_sale_report> SelectAreaSaleReport()
    {
        var r = Sql.DefineDataSet<Area_sale_report>();
        return Sql.From(() => r).Select(() => r).Compile<Area_sale_report>();
    }

    /// <summary>
    /// Area_sale_reportをstaff_idでフィルタリングします
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns></returns>
    private static FluentSelectQuery<Area_sale_report> SelectAreaSaleReportByStaffId(long staffId)
    {
        var r = Sql.DefineDataSet(() => SelectAreaSaleReport());
        return Sql.From(() => r)
            .Where(() => r.sales_staff_id == staffId);
    }

    /// <summary>
    /// Store_sale_reportを全件取得します
    /// </summary>
    /// <returns></returns>
    private static FluentSelectQuery<Store_sale_report> SelectStoreSaleReport()
    {
        var r = Sql.DefineDataSet<Store_sale_report>();
        return Sql.From(() => r).Select(() => r).Compile<Store_sale_report>();
    }

    /// <summary>
    /// スタッフとストアの関連を示すテーブルを取得します
    /// （おおげさ）
    /// </summary>
    /// <returns></returns>
    private static FluentSelectQuery<Area_detail_with_staff> SelectAreaDetailWithStaff()
    {
        var a = Sql.DefineDataSet<Area>();
        var d = Sql.DefineDataSet<Area_detail>();
        return Sql.From(() => d)
            .InnerJoin(() => a, () => d.area_id == a.area_id)
            .Select(() => d)
            .Select(() => new
            {
                a.sales_staff_id
            }).Compile<Area_detail_with_staff>();
    }

    /// <summary>
    /// Store_sale_reportをstaff_idでフィルタリングします
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns></returns>
    private static FluentSelectQuery<Store_sale_report> SelectStoreSaleReportByStaffId(long staffId)
    {
        var r = Sql.DefineDataSet(() => SelectStoreSaleReport());
        return Sql.From(() => r)
            .Exists(SelectAreaDetailWithStaff, x => r.store_id == x.store_id && x.sales_staff_id == staffId);
    }
}

public static class Filtering
{
    /// <summary>
    /// 選択クエリをスタッフIDでフィルタリングします。
    /// フィルタリングできない場合は例外が発生します
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="staffId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static FluentSelectQuery<T> FilterByStaffId<T>(this FluentSelectQuery<T> query, long staffId) where T : IDataRow, new()
    {
        // 列「sales_staff_id」が存在する場合、該当列でフィルタする
        var sales_staff_id = query.SelectClause!.Where(x => x.Alias == "sales_staff_id").FirstOrDefault()?.Alias;
        if (sales_staff_id != null)
        {
            //タイプセーフではないビルド
            //引数のクエリをサブクエリqとして定義する
            var sq = new SelectQuery();
            var (f, q) = sq.From(query).As("q");
            sq.SelectAll(q);

            //列「sales_staff_id」を検索条件にする
            sq.Where(q, sales_staff_id).Equal(sq.AddParameter(":staff_id", staffId));

            //コメントを足す
            sq.AddComment(nameof(FilterByStaffId));

            //タイプセーフに戻す
            return sq.Compile<T>();
        }

        // 列「store_id」が存在する場合、areaテーブル経由でフィルタする
        var store_id = query.SelectClause!.Where(x => x.Alias == "store_id").FirstOrDefault()?.Alias;
        if (store_id != null)
        {
            //タイプセーフではないビルド
            //引数のクエリをサブクエリqとして定義する
            var sq = new SelectQuery();
            var (_, q) = sq.From(query).As("q");
            sq.SelectAll(q);

            //列「store_id」を検索条件にする
            sq.Where(() =>
            {
                // area をstaff_id でフィルタし、
                // area_detail と結合して、store_id に展開する
                var xsq = new SelectQuery();
                var (f, d) = xsq.From("area_detail").As("d");
                var a = f.InnerJoin("area").As("a").On(d, "area_id");
                xsq.Where(a, "sales_staff_id").Equal(xsq.AddParameter(":staff_id", staffId));
                xsq.Where(q, store_id).Equal(d, store_id);
                return xsq.ToExists();
            });

            //コメントを足す
            sq.AddComment($"{nameof(FilterByStaffId)}, column:store_id");

            //タイプセーフに戻す
            return sq.Compile<T>();
        }

        throw new NotImplementedException();
    }
}

public record Area : IDataRow
{
    public long area_id { get; set; }

    public long area_name { get; set; }

    public long sales_staff_id { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}

public record Area_detail : IDataRow
{
    public long area_id { get; set; }

    public long store_id { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}

public record Area_detail_with_staff : IDataRow
{
    public long area_id { get; set; }

    public long store_id { get; set; }

    public long sales_staff_id { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}

public record Area_sale_report : IDataRow
{
    public long area_id { get; set; }
    public long sales_staff_id { get; set; }
    public decimal sale_price { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}

public record Store_sale_report : IDataRow
{
    public long store_id { get; set; }
    public decimal sale_price { get; set; }

    public IDataSet DataSet { get; set; } = null!;
}