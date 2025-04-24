
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusMapApi.Model.Entities;
using System;

using api.Data;

namespace BusMapApi.Controllers
{
    [Route("api/articles")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IWebHostEnvironment _env;

        public ArticleController(ApplicationDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 1️⃣ Lấy danh sách bài viết
        [HttpGet]
        public async Task<IActionResult> GetBaiViet()
        {
            var baiViets = await _context.Articles
                .Include(b => b.AnhBaiViets)
                .Include(b => b.NoiDungBaiViets)
                .AsSplitQuery()  // ⚡ Chia nhỏ truy vấn
                .ToListAsync();
            return Ok(baiViets);
        }


        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] BaiVietQuangCao article)
        {
            if (article == null)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            try
            {
                article.NgayDang = DateTime.Now;

                // Gán mối quan hệ thủ công để tránh lỗi validation
                if (article.NoiDungBaiViets != null)
                {
                    foreach (var nd in article.NoiDungBaiViets)
                    {
                        nd.BaiVietQuangCao = article; // Gán bài viết cha
                    }
                }

                if (article.AnhBaiViets != null)
                {
                    foreach (var anh in article.AnhBaiViets)
                    {
                        anh.BaiVietQuangCao = article; // Gán bài viết cha
                    }
                }

                _context.Articles.Add(article);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetArticleById), new { id = article.ID_BaiViet }, article);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi thêm bài viết: {ex.InnerException?.Message ?? ex.Message}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            var article = await _context.Articles
               .Include(a => a.AnhBaiViets)       // Lấy danh sách ảnh bài viết
               .Include(a => a.NoiDungBaiViets)   // Lấy danh sách nội dung bài viết
               .AsSplitQuery()                    // Giải quyết cảnh báo "MultipleCollectionIncludeWarning"
               .FirstOrDefaultAsync(a => a.ID_BaiViet == id);
            //var article = await _context.Articles.Include(a => a.AnhBaiViets).FirstOrDefaultAsync(a => a.ID_BaiViet == id);
            if (article == null)
            {
                return NotFound("Không tìm thấy bài viết.");
            }
            return Ok(article);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> EditArticle(int id, [FromBody] BaiVietQuangCao updatedArticle)
        {
            if (updatedArticle == null || id != updatedArticle.ID_BaiViet)
            {
                return BadRequest("Dữ liệu không hợp lệ.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingArticle = await _context.Articles
                    .Include(a => a.AnhBaiViets)
                    .Include(a => a.NoiDungBaiViets)
                    .FirstOrDefaultAsync(a => a.ID_BaiViet == id);

                if (existingArticle == null)
                {
                    return NotFound("Bài viết không tồn tại.");
                }

                // Cập nhật thông tin bài viết
                existingArticle.TieuDe = updatedArticle.TieuDe;
                existingArticle.TacGia = updatedArticle.TacGia;
                existingArticle.NgayDang = DateTime.Now;

                // Cập nhật danh sách ảnh
                if (updatedArticle.AnhBaiViets != null)
                {
                    _context.ArticleImages.RemoveRange(existingArticle.AnhBaiViets);

                    foreach (var anh in updatedArticle.AnhBaiViets)
                    {
                        if (anh.ID_Anh == 0) // Chỉ thêm mới nếu chưa có ID
                        {
                            anh.ID_BaiViet = id;
                            _context.ArticleImages.Add(anh);
                        }
                    }
                }

                // Cập nhật nội dung bài viết
                if (updatedArticle.NoiDungBaiViets != null)
                {
                    _context.ArticleContents.RemoveRange(existingArticle.NoiDungBaiViets);

                    foreach (var nd in updatedArticle.NoiDungBaiViets)
                    {
                        if (nd.ID_NoiDung == 0) // Chỉ thêm mới nếu chưa có ID
                        {
                            nd.ID_BaiViet = id;
                            _context.ArticleContents.Add(nd);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Trả về bài viết đã cập nhật
                var updatedResult = await _context.Articles
                    .AsNoTracking()
                    .Include(a => a.AnhBaiViets)
                    .Include(a => a.NoiDungBaiViets)
                    .FirstOrDefaultAsync(a => a.ID_BaiViet == id);

                return Ok(updatedResult);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Lỗi khi cập nhật bài viết: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.AnhBaiViets) // Load danh sách ảnh
                .Include(a => a.NoiDungBaiViets) // Load danh sách nội dung bài viết
                .FirstOrDefaultAsync(a => a.ID_BaiViet == id);

            if (article == null)
            {
                return NotFound("Bài viết không tồn tại.");
            }

            try
            {
                // Xóa nội dung bài viết liên quan
                if (article.NoiDungBaiViets != null && article.NoiDungBaiViets.Any())
                {
                    _context.ArticleContents.RemoveRange(article.NoiDungBaiViets);
                }

                // Xóa ảnh liên quan
                if (article.AnhBaiViets != null && article.AnhBaiViets.Any())
                {
                    _context.ArticleImages.RemoveRange(article.AnhBaiViets);
                }

                // Xóa bài viết
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Bài viết đã bị xóa thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Lỗi khi xóa bài viết: {ex.InnerException?.Message ?? ex.Message}" });
            }
        }

    }
}

