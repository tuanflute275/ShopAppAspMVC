using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopApp.Data;

public partial class ShopAppAspWebContext : DbContext
{
    public ShopAppAspWebContext()
    {
    }

    public ShopAppAspWebContext(DbContextOptions<ShopAppAspWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-PFRQIVL\\SQLEXPRESS01;Initial Catalog=ShopAppAspWeb;Persist Security Info=True;User ID=sa;Password=1234$;Trust Server Certificate=True");


    /*    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sqlserver-container,1433;Database=ShopAppAspWeb;Persist Security Info=True;User ID=sa;Password=Admin@1234;Trust Server Certificate=True"
    );*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Accounts__CB9A1CFF4F198679");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.UserActive)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userActive");
            entity.Property(e => e.UserAddress)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userAddress");
            entity.Property(e => e.UserAvatar)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userAvatar");
            entity.Property(e => e.UserEmail)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userEmail");
            entity.Property(e => e.UserFullName)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userFullName");
            entity.Property(e => e.UserGender)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userGender");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasColumnName("userName");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userPassword");
            entity.Property(e => e.UserPhone)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userPhone");
            entity.Property(e => e.UserRole)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userRole");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blogs__FA0AA72D95DF719A");

            entity.Property(e => e.BlogId).HasColumnName("blogId");
            entity.Property(e => e.BlogDescription)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("ntext")
                .HasColumnName("blogDescription");
            entity.Property(e => e.BlogImage)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("blogImage");
            entity.Property(e => e.BlogName)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("blogName");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__415B03B8F4CAB132");

            entity.Property(e => e.CartId).HasColumnName("cartId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("quantity");
            entity.Property(e => e.TotalAmount)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("totalAmount");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Product).WithMany(p => p.Carts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Carts__productId__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Carts__userId__571DF1D5");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__23CAF1D85090DB53");

            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("categoryName");
            entity.Property(e => e.CategorySlug)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("categorySlug");
            entity.Property(e => e.CategoryStatus)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("categoryStatus");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logs__7839F64DEAEF5831");

            entity.Property(e => e.LogId).HasColumnName("logId");
            entity.Property(e => e.IpAdress)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("ipAdress");
            entity.Property(e => e.Request)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("request");
            entity.Property(e => e.Response)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("ntext")
                .HasColumnName("response");
            entity.Property(e => e.TimeActionRequest)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("timeActionRequest");
            entity.Property(e => e.TimeLogin)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("timeLogin");
            entity.Property(e => e.TimeLogout)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("datetime")
                .HasColumnName("timeLogout");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("userName");
            entity.Property(e => e.WorkTation)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("workTation");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__0809335DCBFC252D");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.OrderAddress)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderAddress");
            entity.Property(e => e.OrderAmount)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderAmount");
            entity.Property(e => e.OrderDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("orderDate");
            entity.Property(e => e.OrderEmail)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderEmail");
            entity.Property(e => e.OrderFullName)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderFullName");
            entity.Property(e => e.OrderNote)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("ntext")
                .HasColumnName("orderNote");
            entity.Property(e => e.OrderPaymentMethods)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderPaymentMethods");
            entity.Property(e => e.OrderPhone)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderPhone");
            entity.Property(e => e.OrderStatus)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderStatus");
            entity.Property(e => e.OrderStatusPayment)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("orderStatusPayment");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__userId__6383C8BA");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__E4FEDE4ADFB7A82D");

            entity.ToTable("OrderDetail");

            entity.Property(e => e.OrderDetailId).HasColumnName("orderDetailId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Price)
                .HasDefaultValue(0.0)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(0)
                .HasColumnName("quantity");
            entity.Property(e => e.TotalMoney)
                .HasDefaultValue(0.0)
                .HasColumnName("totalMoney");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__order__693CA210");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__OrderDeta__produ__6A30C649");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__2D10D16A21E42281");

            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.ProductCategoryId).HasColumnName("productCategoryId");
            entity.Property(e => e.ProductDescription)
                .HasDefaultValueSql("(NULL)")
                .HasColumnType("ntext")
                .HasColumnName("productDescription");
            entity.Property(e => e.ProductImage)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("productImage");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("productName");
            entity.Property(e => e.ProductPrice)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("productPrice");
            entity.Property(e => e.ProductSalePrice)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("productSalePrice");
            entity.Property(e => e.ProductStatus)
                .HasDefaultValueSql("(NULL)")
                .HasColumnName("productStatus");

            entity.HasOne(d => d.ProductCategory).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductCategoryId)
                .HasConstraintName("FK__Products__produc__4CA06362");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
