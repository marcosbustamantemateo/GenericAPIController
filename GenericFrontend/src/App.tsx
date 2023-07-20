import { 
    Refine,
    GitHubBanner, 
    WelcomePage,
    Authenticated, 
} from '@refinedev/core';
import { RefineKbar, RefineKbarProvider } from "@refinedev/kbar";

    import { AuthPage,ErrorComponent
,notificationProvider
,RefineSnackbarProvider
,ThemedLayoutV2} from '@refinedev/mui';

import dataProvider from "@refinedev/simple-rest";
import { CssBaseline, GlobalStyles } from "@mui/material";
import { BrowserRouter, Route, Routes, Outlet } from "react-router-dom";
import routerBindings, { NavigateToResource, CatchAllNavigate, UnsavedChangesNotifier } from "@refinedev/react-router-v6";
import { useTranslation } from "react-i18next";
import { BlogPostList, BlogPostCreate, BlogPostEdit, BlogPostShow } from "pages/blog-posts";
import { CategoryList, CategoryCreate, CategoryEdit, CategoryShow } from "pages/categories";
import { ColorModeContextProvider } from "./contexts/color-mode";
import { Header } from "./components/header";





function App() {
    const { t, i18n } = useTranslation();

    
            const i18nProvider = {
                translate: (key: string, params: object) => t(key, params),
                changeLocale: (lang: string) => i18n.changeLanguage(lang),
                getLocale: () => i18n.language,
            };
            
    
    return (
        <BrowserRouter>
         <RefineKbarProvider>
            <ColorModeContextProvider>
<CssBaseline />
<GlobalStyles styles={{ html: { WebkitFontSmoothing: "auto" } }} />
<RefineSnackbarProvider>

<Refine
            dataProvider={dataProvider("https://localhost:81/api/")}
            resources={[{
                name: "role",
                list: "/Role",
                create: "/Role/Create_Generic",
                edit: "/Role/Update_Generic/:id",
                show: "/Role/ReadByKey_Generic?key=id&value=:id",
                meta: {
                    canDelete: true,
                },
            }, {
                /** 
                 *
                 * Resource is default with default paths, you need to add the components to the paths accordingly.
                 * You can also add custom paths to the resource.
                 * 
                 * Use `<RoleList/>` component at `/role` path.
                 * Use `<RoleCreate/>` component at `/role/create` path.
                 * Use `<RoleEdit/>` component at `/role/edit/:id` path.
                 * Use `<RoleShow/>` component at `/role/show/:id` path.
                 *
                 **/
                name: "role",

                list: "/role",
                create: "/role/create",
                edit: "/role/edit/:id",
                show: "/role/show/:id"
            }]}
        />

            <Refine dataProvider={dataProvider("https://api.fake-rest.refine.dev")}
notificationProvider={notificationProvider}
routerProvider={routerBindings}
i18nProvider={i18nProvider} 
                    resources={[{
                        name: "blog_posts",
                        list: "/blog-posts",
                        create: "/blog-posts/create",
                        edit: "/blog-posts/edit/:id",
                        show: "/blog-posts/show/:id",
                        meta: {
                            canDelete: true,
                        },
                    }, {
                        name: "categories",
                        list: "/categories",
                        create: "/categories/create",
                        edit: "/categories/edit/:id",
                        show: "/categories/show/:id",
                        meta: {
                            canDelete: true,
                        },
                    }, {
                        /** 
                         *
                         * Resource is default with default paths, you need to add the components to the paths accordingly.
                         * You can also add custom paths to the resource.
                         * 
                         * Use `<RoleList/>` component at `/role` path.
                         * Use `<RoleCreate/>` component at `/role/create` path.
                         * Use `<RoleEdit/>` component at `/role/edit/:id` path.
                         * Use `<RoleShow/>` component at `/role/show/:id` path.
                         *
                         **/
                        name: "role",

                        list: "/role",
                        create: "/role/create",
                        edit: "/role/edit/:id",
                        show: "/role/show/:id"
                    }]}
                options={{
                    syncWithLocation: true,
                    warnWhenUnsavedChanges: true,
                }}
            >z
                <Routes>
                    <Route
                        element={(
                                <ThemedLayoutV2
                                    Header={() => <Header isSticky={true} />}
                                >
                                    <Outlet />
                                </ThemedLayoutV2>
                        )}
                    >
                        <Route index element={
                                <NavigateToResource resource="blog_posts" />
                        } />
                        <Route path="/blog-posts">
                            <Route index element={<BlogPostList />} />
                            <Route path="create" element={<BlogPostCreate />} />
                            <Route path="edit/:id" element={<BlogPostEdit />} />
                            <Route path="show/:id" element={<BlogPostShow />} />
                        </Route>
                        <Route path="/categories">
                            <Route index element={<CategoryList />} />
                            <Route path="create" element={<CategoryCreate />} />
                            <Route path="edit/:id" element={<CategoryEdit />} />
                            <Route path="show/:id" element={<CategoryShow />} />
                        </Route>
                        <Route path="*" element={<ErrorComponent />} />
                    </Route>
                </Routes> 

                <RefineKbar />
                <UnsavedChangesNotifier />
            </Refine>
            </RefineSnackbarProvider>


</ColorModeContextProvider>
        </RefineKbarProvider>
        </BrowserRouter>
    );
};

export default App;
