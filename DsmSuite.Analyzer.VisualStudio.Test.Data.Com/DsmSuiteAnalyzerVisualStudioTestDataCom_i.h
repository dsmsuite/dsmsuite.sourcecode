

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* at Sat Aug 29 08:34:33 2020
 */
/* Compiler settings for DsmSuiteAnalyzerVisualStudioTestDataCom.idl:
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

#ifndef __DsmSuiteAnalyzerVisualStudioTestDataCom_i_h__
#define __DsmSuiteAnalyzerVisualStudioTestDataCom_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IComA_FWD_DEFINED__
#define __IComA_FWD_DEFINED__
typedef interface IComA IComA;

#endif 	/* __IComA_FWD_DEFINED__ */


#ifndef __ComA_FWD_DEFINED__
#define __ComA_FWD_DEFINED__

#ifdef __cplusplus
typedef class ComA ComA;
#else
typedef struct ComA ComA;
#endif /* __cplusplus */

#endif 	/* __ComA_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IComA_INTERFACE_DEFINED__
#define __IComA_INTERFACE_DEFINED__

/* interface IComA */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IComA;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("5C80B193-5ABB-437B-9AA7-855BA880FE69")
    IComA : public IDispatch
    {
    public:
    };
    
    
#else 	/* C style interface */

    typedef struct IComAVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IComA * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IComA * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IComA * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IComA * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IComA * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IComA * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IComA * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        END_INTERFACE
    } IComAVtbl;

    interface IComA
    {
        CONST_VTBL struct IComAVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IComA_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IComA_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IComA_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IComA_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IComA_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IComA_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IComA_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IComA_INTERFACE_DEFINED__ */



#ifndef __DsmSuiteAnalyzerVisualStudioTestDataComLib_LIBRARY_DEFINED__
#define __DsmSuiteAnalyzerVisualStudioTestDataComLib_LIBRARY_DEFINED__

/* library DsmSuiteAnalyzerVisualStudioTestDataComLib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_DsmSuiteAnalyzerVisualStudioTestDataComLib;

EXTERN_C const CLSID CLSID_ComA;

#ifdef __cplusplus

class DECLSPEC_UUID("A53A09F4-443C-44D1-9827-76FBC460BF2B")
ComA;
#endif
#endif /* __DsmSuiteAnalyzerVisualStudioTestDataComLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


