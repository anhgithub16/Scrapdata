// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using ScrapingWeb;

Console.OutputEncoding = System.Text.Encoding.Unicode; 
Console.WriteLine("Target! Success");
Console.WriteLine("Scrap data from web");

InputScrap inputDienMayXanh = new InputScrap()
{
    url_base = "https://www.dienmayxanh.com/mua-online-gia-re-hon",
    block_item = "//ul[@class=\"listproduct listing-prod  grid-5\"]//div[@class=\"item\"]",
    block_name = "//a//h3",
    block_historical_cost = "//a//div[@class=\"box-p\"]//p",
    block_real_cost = "//a//strong[@class=\"price\"]"
};

InputScrap inputBalo = new InputScrap()
{
    url_base = "https://bigbag.vn/balo",
    block_item = "//ul[@class=\"products-grid\"]//li//div[@class=\"col-item\"]//div[@class=\"product-image-area-3 them-thuoc-tinh-1\"]",
    block_name = "//div//div[@class=\"info-inner\"]//div[@class=\"item-title\"]//a",
    block_historical_cost = "//div//div[@class=\"info-inner\"]//div[@class=\"item-content\"]//div//p[@class=\"old-price\"]//span[@class=\"price\"]",
    block_real_cost = "//div//div[@class=\"info-inner\"]//div[@class=\"item-content\"]//div//p[@class=\"special-price\"]//span[@class=\"price\"]",
    pageFormat = "?page=[**page**]"
};

InputScrap inputBaloWithXpath = new InputScrap()
{
    url_base = "https://bigbag.vn/balo",
    block_item = "/html/body/section/div/div/div[1]/div[2]/ul/li[1]/div/div[3]",
    block_name = "/html/body/section/div/div/div[1]/div[2]/ul/li[1]/div/div[3]/div/div[1]/div[1]/a",
    block_historical_cost = "/html/body/section/div/div/div[1]/div[2]/ul/li[1]/div/div[3]/div/div[1]/div[2]/div/p[1]/span[2]",
    block_real_cost = "/html/body/section/div/div/div[1]/div[2]/ul/li[1]/div/div[3]/div/div[1]/div[2]/div/p[2]/span",
    pageFormat = "?page=[**page**]",
    pageStart = 1,
    pageEnd = 4
};

Scrap.ScrapProcess(inputBaloWithXpath);

#region nháp

//string url = "https://www.dienmayxanh.com/mua-online-gia-re-hon";
//HtmlWeb web = new HtmlWeb();
//HtmlDocument doc = web.Load(url);
//var nodes = doc.DocumentNode.Descendants("div")
//                                       .Where(node => node.GetAttributeValue("class", "") == "item")
//                                       .ToList();

////HtmlNode nodes = doc.DocumentNode.SelectSingleNode("//html//body//div[6]//section[2]//div//div[2]//ul[2]//div[1]//a//h3");
////.ToList();

//Console.WriteLine("Load doc success! Start get data");

//foreach (var node in nodes)
//{

//    var itemName = node.Descendants("h3").FirstOrDefault();
//    if (itemName != null)
//    {
//        var childNode = node.ChildNodes["a"];
//        var childSingeNode = childNode.SelectNodes(".//div[@class=\"box-p\"]//p");
//        if (childNode != null)
//        {
//            var childChildNode = childNode.ChildNodes["div[@class=\"item-img\"]"];
//        }
//        Console.WriteLine($"Item Name :{itemName.InnerText}");
//    }
//    else
//    {
//        continue;
//    }
//    HtmlNode? boxPriceRoot = node.Descendants("div")
//                                 .Where(n => n.GetAttributeValue("class", "") == "box-p")
//                                 .FirstOrDefault();
//    if (boxPriceRoot != null)
//    {
//        var historicalCost = boxPriceRoot.Descendants("p")
//                                         .Where(n => n.GetAttributeValue("class", "") == "price-old black")
//                                         .FirstOrDefault();
//        if (historicalCost != null)
//        {
//            Console.WriteLine($"Historical Cost: {historicalCost.InnerText}");
//        }
//    }
//    HtmlNode? realCost = node.Descendants("strong")
//                             .Where(n => n.GetAttributeValue("class", "") == "price")
//                             .FirstOrDefault();
//    if (realCost != null)
//    {
//        Console.WriteLine($"Real Cost: {realCost.InnerText}  {realCost.XPath}");
//    }
//    Console.WriteLine("==========================================================");
//}
//int a = 68;


//Scrap.PrintItem(Scrap.ScrapData(inputBalo, null));


//InputScrap objConvert = new InputScrap()
//{
//    url_base = "https://bigbag.vn/balo",
//    block_item = Scrap.ConvertXpath(inputBaloWithXpath.block_item,inputBaloWithXpath.url_base),
//    block_name = Scrap.ConvertXpath(inputBaloWithXpath.block_name, inputBaloWithXpath.url_base),
//    block_historical_cost = Scrap.ConvertXpath(inputBaloWithXpath.block_historical_cost, inputBaloWithXpath.url_base),
//    block_real_cost = Scrap.ConvertXpath(inputBaloWithXpath.block_real_cost, inputBaloWithXpath.url_base)
//};

//Scrap.ScrapProcess(inputBaloWithXpath);

//string xpath = "/html/body/section/div/div/div[1]/div[2]/ul/li[1]/div/div[3]/div/div[1]/div[1]/a";
//string url = "https://bigbag.vn/balo";
//string reC = Scrap.ConvertXpath(xpath, url);
//Console.WriteLine("After convert: " + reC);
//HtmlWeb web = new HtmlWeb();
//HtmlDocument doc = web.Load(url);
//HtmlNode? node = doc.DocumentNode.SelectNodes($".{reC}")?.FirstOrDefault();
//string title = node?.InnerText ?? "";
//Console.WriteLine($"Title: {title}");
#endregion

Console.WriteLine("Success!");
Console.ReadKey();
