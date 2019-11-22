

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Fri Nov 22 12:26:36 2019
 */
/* Compiler settings for DirIDL\IInterface1.idl:
    Oicf, W1, Zp8, env=Win64 (32b run), target_arch=AMD64 8.00.0603 
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

#ifndef __IInterface1_h__
#define __IInterface1_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IInterface1_FWD_DEFINED__
#define __IInterface1_FWD_DEFINED__
typedef interface IInterface1 IInterface1;

#endif 	/* __IInterface1_FWD_DEFINED__ */


#ifndef __CoClass1_FWD_DEFINED__
#define __CoClass1_FWD_DEFINED__

#ifdef __cplusplus
typedef class CoClass1 CoClass1;
#else
typedef struct CoClass1 CoClass1;
#endif /* __cplusplus */

#endif 	/* __CoClass1_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IInterface1_INTERFACE_DEFINED__
#define __IInterface1_INTERFACE_DEFINED__

/* interface IInterface1 */
/* [unique][uuid][object] */ 


EXTERN_C const IID IID_IInterface1;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("E0BFEE0D-D227-4E00-B3C5-6D26D636A0D3")
    IInterface1 : public IUnknown
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE MyMethod( void) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IInterface1Vtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IInterface1 * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IInterface1 * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IInterface1 * This);
        
        HRESULT ( STDMETHODCALLTYPE *MyMethod )( 
            IInterface1 * This);
        
        END_INTERFACE
    } IInterface1Vtbl;

    interface IInterface1
    {
        CONST_VTBL struct IInterface1Vtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IInterface1_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IInterface1_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IInterface1_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IInterface1_MyMethod(This)	\
    ( (This)->lpVtbl -> MyMethod(This) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IInterface1_INTERFACE_DEFINED__ */



#ifndef __Lib1_LIBRARY_DEFINED__
#define __Lib1_LIBRARY_DEFINED__

/* library Lib1 */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_Lib1;

EXTERN_C const CLSID CLSID_CoClass1;

#ifdef __cplusplus

class DECLSPEC_UUID("94617AA0-2D3A-47F5-A9E0-818D2C800416")
CoClass1;
#endif
#endif /* __Lib1_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


