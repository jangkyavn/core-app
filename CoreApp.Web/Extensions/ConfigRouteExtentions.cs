using Microsoft.AspNetCore.Builder;

namespace CoreApp.Web.Extensions
{
    public static class ConfigRouteExtentions
    {
        public static IApplicationBuilder UseRoute(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "blog_tag",
                  template: "bt.{tagId}.html",
                  defaults: new { controller = "Blog", action = "BlogTag" });

                routes.MapRoute(
                  name: "blog_detail",
                  template: "{alias}-b.{id}.html",
                  defaults: new { controller = "Blog", action = "Detail" });

                routes.MapRoute(
                   name: "blog",
                   template: "bai-viet.html",
                   defaults: new { controller = "Blog", action = "Index" });

                routes.MapRoute(
                   name: "product_tag",
                   template: "pt.{tagId}.html",
                   defaults: new { controller = "Product", action = "ProductTag" });

                routes.MapRoute(
                   name: "manager",
                   template: "quan-ly-tai-khoan.html",
                   defaults: new { controller = "Account", action = "Manager" });

                routes.MapRoute(
                   name: "about",
                   template: "gioi-thieu.html",
                   defaults: new { controller = "About", action = "Index" });

                routes.MapRoute(
                   name: "contact",
                   template: "lien-he.html",
                   defaults: new { controller = "Contact", action = "Index" });

                routes.MapRoute(
                   name: "checkout",
                   template: "dat-hang.html",
                   defaults: new { controller = "Cart", action = "Checkout" });

                routes.MapRoute(
                   name: "cart",
                   template: "gio-hang.html",
                   defaults: new { controller = "Cart", action = "Index" });

                routes.MapRoute(
                   name: "search",
                   template: "tim-kiem.html",
                   defaults: new { controller = "Product", action = "Search" });

                routes.MapRoute(
                   name: "register",
                   template: "dang-ky.html",
                   defaults: new { controller = "Account", action = "Register" });

                routes.MapRoute(
                   name: "login",
                   template: "dang-nhap.html",
                   defaults: new { controller = "Account", action = "Login" });

                routes.MapRoute(
                   name: "product_detail",
                   template: "{alias}-p.{id}.html",
                   defaults: new { controller = "Product", action = "Detail" });

                routes.MapRoute(
                   name: "product_catalog",
                   template: "{alias}-c.{id}.html",
                   defaults: new { controller = "Product", action = "Catalog" });

                routes.MapRoute(
                    name: "home",
                    template: "trang-chu.html",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                  name: "areas",
                  template: "{area:exists}/{controller=Account}/{action=Login}/{id?}"
                );
            });

            return app;
        }
    }
}
