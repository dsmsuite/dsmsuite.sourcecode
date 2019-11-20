

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Wed Nov 20 21:22:39 2019
 */
/* Compiler settings for DirIDL\IInterface2.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.00.0603 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __IInterface2_h__
#define __IInterface2_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IInterface2_FWD_DEFINED__
#define __IInterface2_FWD_DEFINED__
typedef interface IInterface2 IInterface2;

#endif 	/* __IInterface2_FWD_DEFINED__ */


#ifndef __CoClass2_FWD_DEFINED__
#define __CoClass2_FWD_DEFINED__

#ifdef __cplusplus
typedef class CoClass2 CoClass2;
#else
typedef struct CoClass2 CoClass2;
#endif /* __cplusplus */

#endif 	/* __CoClass2_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IInterface2_INTERFACE_DEFINED__
#define __IInterface2_INTERFACE_DEFINED__

/* interface IInterface2 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IInterface2;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("7381D5D8-8669-44A8-8A2F-3DFE8070C2E1")
    IInterface2 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MyMethod( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IInterface2Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IInterface2 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IInterface2 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IInterface2 * This);
        
        HRESULT ( STDMETHODCALLTYPE *MyMethod )( 
            IInterface2 * This);
        
        END_INTERFACE
    } IInterface2Vtbl;

    interface IInterface2
    {
        CONST_VTBL struct IInterface2Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IInterface2_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IInterface2_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IInterface2_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IInterface2_MyMethod(This)	\
    ( (This)->lpVtbl -> MyMethod(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IInterface2_INTERFACE_DEFINED__ */



#ifndef __Lib2_LIBRARY_DEFINED__
#define __Lib2_LIBRARY_DEFINED__

/* library Lib2 */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_Lib2;

EXTERN_C const CLSID CLSID_CoClass2;

#ifdef __cplusplus

class DECLSPEC_UUID("94617AA0-2D3A-47F5-A9E0-818D2C800416")
CoClass2;
#endif
#endif /* __Lib2_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


