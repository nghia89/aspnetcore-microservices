namespace Product.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static void UseInfrastructure(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.OAuthClientId("tedu_microservices_swagger");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API");
                c.DisplayRequestDuration();
            });

            app.UseRouting();
            app.UseAuthentication();
            // app.UseHttpsRedirection(); //for production only

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
