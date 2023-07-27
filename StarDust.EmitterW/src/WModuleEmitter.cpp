#include "WModuleEmitter.h"

#include <sstream>
#include <msclr/marshal.h>

namespace StarDust {
    namespace Emitter {
        ModuleEmitter::ModuleEmitter(System::String^ moduleName)
        {
            msclr::interop::marshal_context ctx;
            const char* converted = ctx.marshal_as<const char*>(moduleName);
            _Ptr = new sde::ModuleEmitter(converted, moduleName->Length);
        }

        MethodEmitter^ ModuleEmitter::AddMethod(System::String^ methodName)
        {
            msclr::interop::marshal_context ctx;
            const char* converted = ctx.marshal_as<const char*>(methodName);
            sde::MethodEmitter* ptr = _Ptr->add_method(converted);
            return gcnew MethodEmitter(ptr);
            return nullptr;
        }

        void ModuleEmitter::Emit(StreamWriter^ stream)
        {
            std::stringstream ss;
            _Ptr->write(&ss);

            System::String^ out = gcnew System::String(ss.str().c_str());
            stream->Write(out);
        }
    }
}
