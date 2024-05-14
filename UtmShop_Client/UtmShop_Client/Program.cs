using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProductApiClient
{
    public class UtmShopClient
    {
        private readonly HttpClient client;

        public UtmShopClient()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:5001/api/Category");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var response = await client.GetAsync("/categories");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Category>>(content);
        }

        public async Task<HttpResponseMessage> CreateCategoryAsync(Category model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            return await client.PostAsync("/categories", content);
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            var response = await client.GetAsync($"/categories/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Category>(content);
        }

        public async Task<HttpResponseMessage> DeleteCategoryAsync(int id)
        {
            return await client.DeleteAsync($"/categories/{id}");
        }

        public async Task<List<Product>> GetProductsFromCategoryAsync(int id)
        {
            var response = await client.GetAsync($"/categories/{id}/products");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Product>>(content);
        }

        public async Task<Product> CreateProductInCategoryAsync(int id, Product model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/categories/{id}/products", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Product>(responseContent);
        }

        public async Task<List<Category>> SearchCategoryAsync(string categoryName)
        {
            var response = await client.GetAsync($"/categories/search?categoryName={categoryName}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Category>>(content);
        }

        public async Task<Category> UpdateCategoryAsync(int id, Category model)
        {
            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/categories/{id}", content);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Category>(responseContent);
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }
    }
}
