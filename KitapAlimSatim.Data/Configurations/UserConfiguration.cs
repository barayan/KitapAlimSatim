using KitapAlimSatim.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data.Configurations
{
    class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(e => e.Email).HasMaxLength(100).IsRequired();
            builder.Property(e => e.Password).HasMaxLength(300).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(100).IsRequired();
        }
    }
}
