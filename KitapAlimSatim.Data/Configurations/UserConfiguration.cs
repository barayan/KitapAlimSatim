using KitapAlimSatim.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Configurations
{
    class UserConfiguration : BaseEntityTypeConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(e => e.Email).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Password).HasMaxLength(300).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Address).HasDefaultValue(null);
            builder.Property(e => e.Language).HasMaxLength(2).HasDefaultValue("tr");
            builder.Property(e => e.IsAdmin).HasDefaultValue(false);
            base.Configure(builder);
        }
    }
}

