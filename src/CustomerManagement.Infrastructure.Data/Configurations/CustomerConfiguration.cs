//using CustomerManagement.Domain.Customers;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace CustomerManagement.Infrastructure.Data.Configurations
//{
//    /// <summary>
//    /// Configuração de mapeamento para a entidade Customer no Entity Framework Core.
//    /// </summary>
//    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
//    {
//        public void Configure(EntityTypeBuilder<Customer> builder)
//        {
//            // Define a chave primária da entidade Customer.
//            builder.HasKey(c => c.Id);

//            // Configura a propriedade Name.
//            builder.Property(c => c.Name)
//                .IsRequired()
//                .HasMaxLength(250);

//            // Configura a propriedade Document como um Value Object Owned.
//            // O EF Core mapeará as propriedades de Document diretamente na tabela Customer.
//            //builder.OwnsMany(c => c.Documents, docBuilder =>
//            //{
//            //    docBuilder.Property(d => d.Number)
//            //        .HasColumnName("DocumentNumber")
//            //        .IsRequired()
//            //        .HasMaxLength(14); // CPF ou CNPJ
//            //    docBuilder.Property(d => d.Type)
//            //        .HasColumnName("DocumentType")
//            //        .IsRequired();
//            //    docBuilder.HasIndex(d => d.Number).IsUnique(); // Garante unicidade do documento
//            //});

//            // Configura a coleção de Emails como Value Objects Owned.
//            // Cada Email será mapeado para uma tabela separada (CustomerEmails).
//            builder.OwnsMany(c => c.Documents, docBuilder =>
//            {
//                docBuilder.WithOwner().HasForeignKey("CustomerId"); // Chave estrangeira para Customer
//                docBuilder.HasKey("Id"); // Chave primária para a tabela de emails
//                docBuilder.Property(e => e.Type)
//                    .IsRequired()
//                    .HasMaxLength(250);
//                docBuilder.Property(e => e.Number)
//                    .IsRequired();
//                docBuilder.ToTable("CustomerDocuments"); // Nome da tabela para emails
//            });

//            // Configura a coleção de Emails como Value Objects Owned.
//            // Cada Email será mapeado para uma tabela separada (CustomerEmails).
//            builder.OwnsMany(c => c.Emails, emailBuilder =>
//            {
//                emailBuilder.WithOwner().HasForeignKey("CustomerId"); // Chave estrangeira para Customer
//                emailBuilder.HasKey("Id"); // Chave primária para a tabela de emails
//                emailBuilder.Property(e => e.Address)
//                    .IsRequired()
//                    .HasMaxLength(250);
//                emailBuilder.Property(e => e.IsPrimary)
//                    .IsRequired();
//                emailBuilder.ToTable("CustomerEmails"); // Nome da tabela para emails
//            });

//            // Configura a coleção de Phones como Value Objects Owned.
//            // Cada Phone será mapeado para uma tabela separada (CustomerPhones).
//            builder.OwnsMany(c => c.Phones, phoneBuilder =>
//            {
//                phoneBuilder.WithOwner().HasForeignKey("CustomerId"); // Chave estrangeira para Customer
//                phoneBuilder.HasKey("Id"); // Chave primária para a tabela de telefones
//                phoneBuilder.Property(p => p.CountryCode)
//                    .IsRequired()
//                    .HasMaxLength(5);
//                phoneBuilder.Property(p => p.AreaCode)
//                    .IsRequired()
//                    .HasMaxLength(5);
//                phoneBuilder.Property(p => p.Number)
//                    .IsRequired()
//                    .HasMaxLength(15);
//                phoneBuilder.Property(p => p.IsPrimary)
//                    .IsRequired();
//                phoneBuilder.ToTable("CustomerPhones"); // Nome da tabela para telefones
//            });

//            // Configura a coleção de Addresses como Value Objects Owned.
//            // Cada Address será mapeado para uma tabela separada (CustomerAddresses).
//            builder.OwnsMany(c => c.Addresses, addressBuilder =>
//            {
//                addressBuilder.WithOwner().HasForeignKey("CustomerId"); // Chave estrangeira para Customer
//                addressBuilder.HasKey("Id"); // Chave primária para a tabela de endereços
//                addressBuilder.Property(a => a.Street)
//                    .IsRequired()
//                    .HasMaxLength(200);
//                addressBuilder.Property(a => a.Number)
//                    .HasMaxLength(50);
//                addressBuilder.Property(a => a.Complement)
//                    .HasMaxLength(100);
//                addressBuilder.Property(a => a.Neighborhood)
//                    .HasMaxLength(100);
//                addressBuilder.Property(a => a.City)
//                    .IsRequired()
//                    .HasMaxLength(100);
//                addressBuilder.Property(a => a.State)
//                    .IsRequired()
//                    .HasMaxLength(50);
//                addressBuilder.Property(a => a.ZipCode)
//                    .IsRequired()
//                    .HasMaxLength(10);
//                addressBuilder.Property(a => a.Country)
//                    .IsRequired()
//                    .HasMaxLength(100);
//                addressBuilder.Property(a => a.IsPrimary)
//                    .IsRequired();
//                addressBuilder.ToTable("CustomerAddresses"); // Nome da tabela para endereços
//            });

//            // Ignora a propriedade DomainEvents, pois não será persistida.
//            // Isso é útil se você tiver uma lista de eventos de domínio na sua Aggregate Root.
//            // builder.Ignore(c => c.DomainEvents);
//        }
//    }
//}