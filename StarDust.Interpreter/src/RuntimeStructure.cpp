#include "stdafx.h"

#include "RuntimeStructure.h"

#include "Logger.h"

int sdi::RuntimeStructure::append(RuntimeStructure* other)
{
    if (other == NULL)
        return -1;

    for (auto& func : other->m_Functions)
    {
        std::string funcName = func.first;
        if (func.second == m_Functions.get_function(funcName))
        {
            _SDVM_LOGEF("Could not append function '{}' because it already exists!", func.first);
            continue;
        }

        m_Functions.insert_function(func.second);
    }

    other->reset();
    return 0;
}

void sdi::RuntimeStructure::reset()
{
    m_Functions.clear();
}
