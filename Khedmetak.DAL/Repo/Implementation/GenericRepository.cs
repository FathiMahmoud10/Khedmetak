using Khedmetak.Core.Data;
using Khedmetak.DAL.Entities;
using Khedmetak.DAL.Entities.Base;
using Khedmetak.DAL.Repo.shared;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Khedmetak.DAL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        #region Fields & Constructor
        // الـ DbContext الأساسي والـ DbSet الخاص بالـ Entity الحالي
        // بيتحقنوا مرة واحدة في الكونستركتور ويتم استخدامهم في كل عمليات القراءة/الكتابة تحت
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        #endregion


        #region Get / Retrieve All
        // عمليات جلب كل الـ Records من الجدول
        // فيه نسختين: واحدة بسيطة، وواحدة بتقبل Includes لعمل Eager Loading للعلاقات (Navigation Properties)
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);
            return await query.ToListAsync();
        }
        #endregion


        #region Get By Id
        // جلب Entity واحد بالـ Id بتاعه
        // النسخة الأولى بتستخدم FindAsync (بتدور في الـ Change Tracker الأول قبل الداتابيز، أسرع للـ PK lookup)
        // النسخة التانية بتستخدم Query عادي عشان تقدر تعمل Include مع الـ Id
        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(e => e.Id == id);
        }
        #endregion


        #region Find / Search (Multiple Results)
        // عمليات البحث اللي بترجع أكتر من نتيجة حسب شرط (Predicate) بيتحدد وقت الاستدعاء
        // FindAsyncr: بحث بسيط بشرط واحد بس (ملحوظة: فيه خطأ إملائي في الاسم "Asyncr")
        // FindAllByAsync: نفس الفكرة بس مع دعم الـ Includes للعلاقات
        public async Task<IEnumerable<T>> FindAsyncr(Expression<Func<T, bool>> predicate)
            => await _dbSet.Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> FindAllByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.Where(predicate).ToListAsync();
        }
        #endregion


        #region Find One (Single Result)
        // عمليات البحث اللي بترجع Entity واحد بس مطابق للشرط (أو null لو مفيش تطابق)
        // بنفس فكرة الـ overloads: نسخة بسيطة، ونسخة بتدعم Includes
        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.FirstOrDefaultAsync(predicate);
        }
        #endregion


        #region Write Operations (Add / Update / Delete)
        // عمليات التعديل على الـ DbSet (Add / Update / Delete)
        // ملحوظة: العمليات دي بتعمل Track للتغيير بس مش بتعمل SaveChanges
        // يعني لازم يتم استدعاء SaveChangesAsync من الـ UnitOfWork أو من الـ Service بعد النداء على أي منهم
        public void Add(T entity) => _dbSet.Add(entity);

        public void Update(T entity) => _dbSet.Update(entity);

        public void Delete(T entity) => _dbSet.Remove(entity);
        #endregion


        #region Not Implemented (Legacy / Placeholder)
        // ميثودز موجودة في الـ Interface بس لسه معمولهاش implementation فعلي
        // بترمي NotImplementedException لو اتنادى عليها دلوقتي
        // FindAsync الأولى كمان مربوطة بـ Type محدد (UserDocument) بدل الـ Generic Type، وده مخالف لباقي منطق الكلاس
        public Task<IEnumerable<UserDocument>> FindAsync(Expression<Func<UserDocument, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}