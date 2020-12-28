using KitapAlimSatim.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Configurations
{
    class BookConfiguration : BaseEntityTypeConfiguration<Book>
    {
        public override void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Author).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Publisher).HasMaxLength(100).IsRequired();
            base.Configure(builder);
        }
    }
}
