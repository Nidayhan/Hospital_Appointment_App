using HospitalAppointment_domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HospitalAppointment_core.Interfaces
{
    public interface IDepartmentRepository
    {
        Department GetDepartmentById(int departmentId);
        void UpdateDepartment(Department department);
    }
}
