using _19T1021188.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _19T1021188.DomainModels;
using _19T1021188.Web.Models;
using System.IO;

namespace _19T1021188.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    [RoutePrefix("product")]
    public class ProductController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private const int PAGE_SIZE = 5;
        private const string PRODUCT_SEARCH = "ProductCondition";
        /// <summary>
        /// Tìm kiếm, hiển thị mặt hàng dưới dạng phân trang
        /// </summary>
        /// <returns></returns>
        /*public ActionResult Index(int page = 1, string searchValue = "", int categoryID = 0, int supplierID = 0)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(page, PAGE_SIZE, searchValue, categoryID, supplierID, out rowCount);
            int pageCount = rowCount / PAGE_SIZE;
            if (rowCount % PAGE_SIZE != 0)
                pageCount++;
            ViewBag.Page = page;
            ViewBag.RowCount = rowCount;
            ViewBag.PageCount = pageCount;
            ViewBag.SearchValue = searchValue;

            return View(data);//Truyền dữ liệu bằng Model
        }*/
        public ActionResult Index()
        {
            Models.ProductPaginationSearchInput condition = Session[PRODUCT_SEARCH] as Models.ProductPaginationSearchInput;
            if (condition == null)
            {
                condition = new Models.ProductPaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                };
            }
            return View(condition);
        }

        public ActionResult Search(Models.ProductPaginationSearchInput condition)
        {
            int rowCount = 0;
            var data = ProductDataService.ListProducts(condition.Page, condition.PageSize, condition.SearchValue, condition.CategoryID, condition.SupplierID, out rowCount);
            Models.ProductSearchOutput result = new Models.ProductSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session[PRODUCT_SEARCH] = condition;
            return View(result);
        }

        /// <summary>
        /// Tạo mặt hàng mới
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }
        /// <summary>
        /// Cập nhật thông tin mặt hàng, 
        /// Hiển thị danh sách ảnh và thuộc tính của mặt hàng, điều hướng đến các chức năng
        /// quản lý ảnh và thuộc tính của mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        public ActionResult Edit(int id = 0)
        {
            var data = new ProductEditModel();
            var data1 = ProductDataService.Get(id);
            var data2 = ProductDataService.ListAttributes(id);
            var data3 = ProductDataService.ListPhotos(id);

            data.ProductID = data1.ProductID;
            data.ProductName = data1.ProductName;
            data.SupplierID = data1.SupplierID;
            data.CategoryID = data1.CategoryID;
            data.Unit = data1.Unit;
            data.Price = data1.Price;
            data.Photo = data1.Photo;
            data.LAttribute = data2;
            data.LPhoto = data3;
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }
        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>        
        public ActionResult Delete(int id = 0)
        {
            if (id <= 0)
                return RedirectToAction("Index");
            if (Request.HttpMethod == "POST")
            {
                ProductDataService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            var data = ProductDataService.Get(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

        /// <summary>
        /// Các chức năng quản lý ảnh của mặt hàng
        /// </summary>
        /// <param name="method"></param>
        /// <param name="productID"></param>
        /// <param name="photoID"></param>
        /// <returns></returns>
        [Route("photo/{method?}/{productID?}/{photoID?}")]
        public ActionResult Photo(string method = "add", int productID = 0, long photoID = 0)
        {
            ViewBag.ProductID = productID;
            switch (method)
            {
                case "add":
                    ProductPhoto a = new ProductPhoto
                    {
                        PhotoID = 0,
                        ProductID = productID
                    };
                    ViewBag.Title = "Bổ sung ảnh";
                    return View(a);
                case "edit":
                    ViewBag.PhotoID = photoID;
                    var data = ProductDataService.GetPhoto(photoID);
                    ViewBag.Title = "Thay đổi ảnh";
                    return View(data);
                case "delete":
                    ProductDataService.DeletePhoto(photoID);
                    return RedirectToAction($"Edit/{productID}"); //return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Các chức năng quản lý thuộc tính của mặt hàng
        /// </summary>
        /// <param name="method"></param>
        /// <param name="productID"></param>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        [Route("attribute/{method?}/{productID}/{attributeID?}")]
        public ActionResult Attribute(string method = "add", int productID = 0, long attributeID = 0)
        {
            ViewBag.ProductID = productID;
            switch (method)
            {
                case "add":
                    ProductAttribute a = new ProductAttribute
                    {
                        AttributeID = 0,
                        ProductID = productID
                    };
                    ViewBag.Title = "Bổ sung thuộc tính";
                    return View(a);
                case "edit":
                    ViewBag.AttributeID = attributeID;
                    var data = ProductDataService.GetAttribute(attributeID);
                    ViewBag.Title = "Thay đổi thuộc tính";
                    return View(data);
                case "delete":
                    ProductDataService.DeleteAttribute(attributeID);
                    return RedirectToAction($"Edit/{productID}"); //return RedirectToAction("Edit", new { productID = productID });
                default:
                    return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public ActionResult Save(Product data, HttpPostedFileBase uploadPhoto)
        {
            data.Photo = uploadPhoto.ToString();

            if (string.IsNullOrWhiteSpace(data.ProductName))
                ModelState.AddModelError(nameof(data.ProductName), "Tên mặt hàng không được để trống!");
            if (data.CategoryID == 0)
                ModelState.AddModelError(nameof(data.CategoryID), "Loại hàng không được để trống!");
            if (data.SupplierID == 0)
                ModelState.AddModelError(nameof(data.SupplierID), "Nhà cung cấp không được để trống!");
            if (string.IsNullOrEmpty(data.Photo))
                ModelState.AddModelError(nameof(data.Photo), "Hình ảnh không được trống!");
            data.Unit = data.Unit ?? "";
            data.Price = data.Price;


            if (ModelState.IsValid == false) // hoặc !ModelState.IsValid
            {
                if (data.ProductID == 0)
                    return View("Create",data);
                else
                {
                    var dt = new ProductEditModel();
                    var data1 = ProductDataService.Get(data.ProductID);
                    var data2 = ProductDataService.ListAttributes(data.ProductID);
                    var data3 = ProductDataService.ListPhotos(data.ProductID);

                    dt.ProductID = data1.ProductID;
                    dt.ProductName = data1.ProductName;
                    dt.SupplierID = data1.SupplierID;
                    dt.CategoryID = data1.CategoryID;
                    dt.Unit = data1.Unit;
                    dt.Price = data1.Price;
                    dt.Photo = data1.Photo;
                    dt.LAttribute = data2;
                    dt.LPhoto = data3;
                    return View("Edit",dt);
                }
                    
            }

            if (uploadPhoto != null)
            {
                string path = Server.MapPath("~/Images/Products");
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(path, fileName);
                uploadPhoto.SaveAs(filePath);
                data.Photo = $"Images/Products/{fileName}";
            }

            if (data.ProductID == 0)
                ProductDataService.AddProduct(data);
            else
                ProductDataService.UpdateProduct(data);
            return RedirectToAction("Index");
        }

        public ActionResult SaveAttribute (ProductAttribute data)
        {
            if (string.IsNullOrWhiteSpace(data.AttributeName))
                ModelState.AddModelError(nameof(data.AttributeName), "Tên thuộc tính không được trống!");
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không được trống!");
            if (data.DisplayOrder < 1)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải từ 1 trở lên!");

            if (ModelState.IsValid == false)
            {
                ViewBag.Title = data.AttributeID == 0 ? "Bổ sung thuộc tính" : "Thay đổi thuộc tính";
                return View("Attribute", data);
            }

            if (data.AttributeID == 0)
                ProductDataService.AddAttribute(data);
            else
                ProductDataService.UpdateAttribute(data);
            return RedirectToAction($"Edit/{data.ProductID}");
        }

        public ActionResult SavePhoto(ProductPhoto data, HttpPostedFileBase uploadPhoto)
        {
            if (string.IsNullOrEmpty(data.Photo))
                ModelState.AddModelError(nameof(data.Photo), "Hình ảnh không được trống!");
            if (string.IsNullOrWhiteSpace(data.DisplayOrder.ToString()))
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị không được trống!");
            if (data.DisplayOrder < 1)
                ModelState.AddModelError(nameof(data.DisplayOrder), "Thứ tự hiển thị phải từ 1 trở lên!");

            if (ModelState.IsValid == false)
            {
                ViewBag.Title = data.PhotoID == 0 ? "Bổ sung ảnh" : "Thay đổi ảnh";
                return View("Photo", data);
            }

            if (uploadPhoto != null)
            {
                string path = Server.MapPath("~/Images/Products");
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}";
                string filePath = System.IO.Path.Combine(path, fileName);
                uploadPhoto.SaveAs(filePath);
                data.Photo = $"Images/Products/{fileName}";
            }

            if (data.PhotoID == 0)
                ProductDataService.AddPhoto(data);
            else
                ProductDataService.UpdatePhoto(data);
            return RedirectToAction($"Edit/{data.ProductID}");
        }
    }
}