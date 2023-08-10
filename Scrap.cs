using HtmlAgilityPack;

namespace ScrapingWeb
{
    public class Scrap
    {
        public static void ScrapProcess(InputScrap input)
        {
            try
            {
                //handler : load 1 web page default(page = 1) để convert xpath to treeClass
                //load web
                HtmlDocument docDefault = LoadWeb(input.url_base);
                //xử lý input - convert => kết quả là template treeClass - dùng để scrap dữ liệu
                InputScrap inputConvert = ConvertXpath(input, docDefault);
                //xử lý cào dữ liệu - per page -- xử lý thêm load theo trang 
                //sử dụng template là treeClass từ func Convert để scrap dữ liệu , theo trang(all trang -- sử dụng loop)
                if(string.IsNullOrEmpty(input.pageFormat) || input.pageFormat == "")
                {
                    List<OutputScrap> outPuts = ScrapData(inputConvert, docDefault);
                    // print dữ liệu -- lưu vào db
                    PrintItem(outPuts);
                }
                else
                {
                    //validate pageStart và pageEnd
                    if(input.pageStart > input.pageEnd)
                    {
                        throw new Exception("Invalid: pageStart không được lớn hơn pageEnd!!");
                    }
                    else
                    {
                        //loop to scrap per page
                        for(int i = input.pageStart; i <= input.pageEnd; i++)
                        {
                            string urlPerPage = $"{input.url_base}{input.pageFormat}".Replace("[**page**]", i.ToString());
                            HtmlDocument docPerPage = LoadWeb(urlPerPage); // load per page
                            Console.WriteLine("Start scarp page " + i);
                            List<OutputScrap> outPuts = ScrapData(inputConvert, docPerPage);
                            // print dữ liệu -- lưu vào db
                            PrintItem(outPuts);
                            Console.WriteLine($"Scrap success!!Add {outPuts.Count()} item ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in ScrapProcess: " + ex.Message);
            }
        }
        public static HtmlDocument LoadWeb(string url)
        {
            HtmlDocument doc = new HtmlDocument();
            try
            {
                if (string.IsNullOrEmpty(url) || url == "")
                {
                    throw new Exception("Url_base is invalid: Url is not null or empty!");
                }
                HtmlWeb web = new HtmlWeb();
                doc = web.Load(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in LoadWeb: " + ex.Message);
            }
            return doc;
        }
        public static List<OutputScrap> ScrapData(InputScrap input, HtmlDocument? doc)
        {

            List<OutputScrap> outputs = new List<OutputScrap>();
            try
            {
                if(doc == null)
                {
                    doc = LoadWeb(input.url_base);
                }

                if (!string.IsNullOrEmpty(input.block_item))
                {
                    //access to block item - where representation for infor of item
                    var lst_node_items = doc.DocumentNode.SelectNodes(input.block_item)?.ToList();
                    if (lst_node_items != null && lst_node_items.Any())
                    {
                        foreach (var node in lst_node_items)
                        {
                            OutputScrap scrap = new OutputScrap();
                               
                            //ten_san_pham
                            if (!string.IsNullOrEmpty(input.block_name))
                            {
                                HtmlNode? node_name = node.SelectNodes($".{input.block_name}")?.FirstOrDefault();
                                if (node_name != null)
                                {
                                    scrap.ten_san_pham = node_name.InnerText;
                                }
                            }
                                
                            // gia goc
                            if (!string.IsNullOrEmpty(input.block_historical_cost))
                            {
                                HtmlNode? node_historical_cost = node.SelectNodes($".{input.block_historical_cost}")?.FirstOrDefault();
                                if (node_historical_cost != null)
                                {
                                    scrap.gia_goc = node_historical_cost.InnerText;
                                }
                            }
                               
                            //gia thuc te
                            if (!string.IsNullOrEmpty(input.block_real_cost))
                            {
                                HtmlNode? node_real_cost = node.SelectNodes($".{input.block_real_cost}")?.FirstOrDefault();
                                if (node_real_cost != null)
                                {
                                    scrap.gia_thuc_te = node_real_cost.InnerText;
                                }
                            }

                            outputs.Add(scrap);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            //if (is_success == true)
            //    PrintItem(output);

            return outputs;
        }
        public static void PrintItem(List<OutputScrap> data)
        {
            try
            {
                if (data != null && data.Any())
                {
                    foreach (OutputScrap item in data)
                    {
                        Console.WriteLine($"Id : {item.Id}");
                        Console.WriteLine($"Tên sản phẩm : {item.ten_san_pham}");
                        Console.WriteLine($"Giá gốc :{item.gia_goc}");
                        Console.WriteLine($"Giá sau giảm giá: {item.gia_thuc_te}");
                        Console.WriteLine("===========================================================");
                    }
                }
                else
                {
                    Console.WriteLine("No data!!!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

        }
        public static InputScrap ConvertXpath(InputScrap input, HtmlDocument? doc)
        {
            InputScrap scrap = new InputScrap();
            try
            {
                if(doc == null) // n cách sử dụng
                {
                    //load web
                    doc = LoadWeb(input.url_base);
                }
                if(!string.IsNullOrEmpty(input.block_item) && input.block_item != "") // theo luồng thì đây là parent elenment: contain all data to scrap
                {
                    scrap.url_base = input.url_base;
                    scrap.block_item = ProcessConvertXpath(input.block_item, doc);//bounder  => deepDefault = 5
                }
                else
                {
                    throw new Exception("Invalid!block_item is not null.It is bound of infor so it is required");
                }
                int countParent = input.block_item.Split("/").Count();
                //block_name
                if(!string.IsNullOrEmpty(input.block_name) && input.block_name != "")
                {
                    //check quan hệ cha con của block_name và block_item
                    if (input.block_name.StartsWith(input.block_item))
                    {
                        //caculate deep convert of child
                        int countChild = input.block_name.Split("/").Count();
                        int deep = countChild - countParent;
                        scrap.block_name = ProcessConvertXpath(input.block_name, doc, deep);
                    }
                    else
                    {
                        throw new Exception("Invalid!block_name not in block_item");
                    }
                }

                //block_real_cost
                if (!string.IsNullOrEmpty(input.block_real_cost) && input.block_real_cost != "")
                {
                    //check quan hệ cha con của block_name và block_item
                    if (input.block_real_cost.StartsWith(input.block_item))
                    {
                        //caculate deep convert of child
                        int countChild = input.block_real_cost.Split("/").Count();
                        int deep = countChild - countParent;
                        scrap.block_real_cost = ProcessConvertXpath(input.block_real_cost, doc, deep);
                    }
                    else
                    {
                        throw new Exception("Invalid!block_real_cost not in block_item");
                    }
                }

                //block_historical_cost
                if (!string.IsNullOrEmpty(input.block_historical_cost) && input.block_historical_cost != "")
                {
                    //check quan hệ cha con của block_name và block_item
                    if (input.block_historical_cost.StartsWith(input.block_item))
                    {
                        //caculate deep convert of child
                        int countChild = input.block_historical_cost.Split("/").Count();
                        int deep = countChild - countParent;
                        scrap.block_historical_cost = ProcessConvertXpath(input.block_historical_cost, doc, deep);
                    }
                    else
                    {
                        throw new Exception("Invalid!block_historical_cost not in block_item");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ConvertXpath: {ex.Message}");
            }
            return scrap;
        }
        public static string ProcessConvertXpath(string xpath, HtmlDocument doc, int deepDef = 5)
        {
            try
            {
                if (!string.IsNullOrEmpty(xpath))
                {
                    string cov = "";
                    Console.WriteLine("Xpath: " + xpath);
                   
                    var temp = xpath.Split("/").ToList();

                    int deep = 0;
                    while (temp != null && temp.Count > 0)
                    {
                        int count = temp.Count;
                        string merge = string.Join("/", temp);
                        HtmlNode? nodeGet = doc.DocumentNode.SelectNodes(merge)?.FirstOrDefault();
                        if (nodeGet == null)
                        {
                            Console.WriteLine("Not found xpath");
                            return "";
                        }
                        string? getAttributeValue = nodeGet.GetAttributeValue("class", "");

                        string tempClass = (!string.IsNullOrEmpty(getAttributeValue) && getAttributeValue != "") ? $"[@class=\"{getAttributeValue}\"]" : "";
                        cov = $"//{nodeGet.Name}{tempClass}{cov}";
                        //remove last index of list
                        temp = temp.Take(count - 1).ToList();
                        deep++;
                        if (deep == deepDef) // độ sâu lấy className trong tree xpath
                        {
                            break;
                        }
                    }
                    Console.WriteLine("Cov: " + cov);
                    return cov;
                }
                Console.WriteLine("Xpath is null");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in ProcessConvertXpath: {ex.Message}");
            }
            return "";
        }

    }
    public class InputScrap
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string url_base { get; set; }
        public string block_item { get; set; }
        public string block_name { get; set; }
        public string block_historical_cost { get; set; }
        public string block_real_cost { get; set; }
        public string pageFormat { get; set; } // trong chuỗi có chứa 1 format : [**page**] => replace thành trang muốn scrap
        public int pageStart { get; set; } // page bắt dầu scrap
        public int pageEnd { get; set; } // page kết thúc scrap
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
    public class OutputScrap
    {
        public Guid Id { get; set; } = Guid.NewGuid();
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string ten_san_pham { get; set; }
        public string gia_goc { get; set; } // giá trc khi giảm giá : historical cost
        public string gia_thuc_te { get; set; } // giá sau khi giảm giá : real cost
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
