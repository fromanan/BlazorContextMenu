"use strict";

export const blazorContextMenu = function(blazorContextMenu)
{
    const setOffset = function(menu, topOffset = 0, leftOffset = 0) {
        const topOverflownPixels = menu.offsetTop + menu.clientHeight - window.innerHeight;
        if (topOverflownPixels > 0)
            menu.style.top = (menu.offsetTop - topOffset) + "px";

        const leftOverflownPixels = menu.offsetLeft + menu.clientWidth - window.innerWidth;
        if (leftOverflownPixels > 0)
            menu.style.left = (menu.offsetLeft - menu.clientWidth - (leftOffset > 0 ? leftOffset : leftOverflownPixels)) + "px";
    };

    let closest = null;
    if (window.Element && !Element.prototype.closest)
    {
        closest = function(el, s)
        {
            let matches = (el.document || el.ownerDocument).querySelectorAll(s), i;
            do
            {
                i = matches.length;
                while (--i >= 0 && matches.item(i) !== el)
                {
                    
                }
            } 
            while ((i < 0) && (el = el.parentElement));
            
            return el;
        };
    }
    else
    {
        closest = (el, s) => el.closest(s);
    }

    const openMenus = [];

    //Helper functions
    //========================================
    function guid()
    {
        function s4()
        {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }

    function findFirstChildByClass(element, className)
    {
        let foundElement = null;

        function recurse(element, className, found)
        {
            for (let i = 0; i < element.children.length && !found; i++)
            {
                const el = element.children[i];
                
                if (el.classList.contains(className))
                {
                    found = true;
                    foundElement = element.children[i];
                    break;
                }
                
                if (found)
                    break;
                
                recurse(element.children[i], className, found);
            }
        }

        recurse(element, className, false);

        return foundElement;
    }

    function findAllChildrenByClass(element, className)
    {
        const foundElements = [];

        function recurse(element, className)
        {
            for (let i = 0; i < element.children.length; i++)
            {
                const el = element.children[i];
                if (el.classList.contains(className))
                    foundElements.push(element.children[i]);
                recurse(element.children[i], className);
            }
        }

        recurse(element, className);
        return foundElements;
    }

    function removeItemFromArray(array, item)
    {
        for (let i = 0; i < array.length; i++)
        {
            if (array[i] === item)
                array.splice(i, 1);
        }
    }

    let sleepUntil = async (f, timeoutMs) => 
    {
        return new Promise((resolve, reject) =>
        {
            const timeWas = new Date();
            const wait = setInterval(() =>
            {
                if (f())
                {
                    clearInterval(wait);
                    resolve();
                }
                else if (new Date() - timeWas > timeoutMs)
                {
                    clearInterval(wait);
                    reject();
                }
            }, 20);
        });
    }

    //===========================================

    let menuHandlerReference = null;

    blazorContextMenu.SetMenuHandlerReference = function(dotnetRef)
    {
        if (!menuHandlerReference)
            menuHandlerReference = dotnetRef;
    }

    const addToOpenMenus = function(menu, menuId, target)
    {
        const instanceId = guid();
        
        openMenus.push(
        {
            id: menuId,
            target: target,
            instanceId: instanceId
        });
        
        menu.dataset["instanceId"] = instanceId;
    };

    blazorContextMenu.ManualShow = function(menuId, x, y)
    {
        const menu = document.getElementById(menuId);
        if (!menu)
            throw new Error("No context menu with id '" + menuId + "' was found");
        
        addToOpenMenus(menu, menuId, null);
        showMenuCommon(menu, menuId, x, y, null, null);
    }

    blazorContextMenu.OnContextMenu = function(args, menuId, stopPropagation)
    {
        const menu = document.getElementById(menuId);
        if (!menu)
            throw new Error("No context menu with id '" + menuId + "' was found");
        
        addToOpenMenus(menu, menuId, args.target);
        const triggerDotnetRef = JSON.parse(args.currentTarget.dataset["dotnetref"]);
        showMenuCommon(menu, menuId, args.x, args.y, args.target, triggerDotnetRef);
        args.preventDefault();
        
        if (stopPropagation)
            args.stopPropagation();
        
        return false;
    };

    const showMenuCommon = function(menu, menuId, x, y, target, triggerDotnetRef)
    {
        return blazorContextMenu.Show(menuId, x, y, target, triggerDotnetRef).then(async () =>
        {
            // Wait until the menu has spawned so clientWidth and offsetLeft report correctly
            await sleepUntil(() => menu.clientWidth > 0, 1000);

            //check for overflow
            setOffset(menu, menu.clientHeight)
        });
    };

    // TODO: Resolve conflict
    const showMenuCommon = function (menu, menuId, x, y, target, triggerDotnetRef)
    {
        //Wait until the menu has spawned so clientWidth and offsetLeft report correctly
        return blazorContextMenu.Show(menuId, x, y, target, triggerDotnetRef)
        .then(function () { return sleepUntil(function () { return menu.clientWidth > 0 }, 1000); })
        .then(function ()
        {
            //check for overflow
            var leftOverflownPixels = menu.offsetLeft + menu.clientWidth - window.innerWidth;
            if (leftOverflownPixels > 0) {
                menu.style.left = (menu.offsetLeft - menu.clientWidth) + "px";
            }

            var topOverflownPixels = menu.offsetTop + menu.clientHeight - window.innerHeight;
            if (topOverflownPixels > 0) {
                menu.style.top = (menu.offsetTop - menu.clientHeight) + "px";
            }

            //openingMenu = false;
        });
    }

    blazorContextMenu.Init = function()
    {
        document.addEventListener("mouseup", args =>
        {
            handleAutoHideEvent(args, "mouseup");
        });

        document.addEventListener("mousedown", args =>
        {
            handleAutoHideEvent(args, "mousedown");
        });

        function handleAutoHideEvent(args, autoHideEvent)
        {
            if (openMenus.length > 0)
            {
                for (let i = 0; i < openMenus.length; i++)
                {
                    const currentMenu = openMenus[i];
                    const menuElement = document.getElementById(currentMenu.id);
                    if (menuElement && menuElement.dataset["autohide"] === "true" && menuElement.dataset["autohideevent"] === autoHideEvent)
                    {
                        const clickedInsideMenu = menuElement.contains(args.target);
                        if (!clickedInsideMenu)
                            blazorContextMenu.Hide(currentMenu.id);
                    }
                }
            }
        }

        window.addEventListener('resize', () =>
        {
            if (openMenus.length <= 0)
                return;
            
            for (let i = 0; i < openMenus.length; i++)
            {
                const currentMenu = openMenus[i];
                const menuElement = document.getElementById(currentMenu.id);
                if (menuElement && menuElement.dataset["autohide"] === "true")
                    blazorContextMenu.Hide(currentMenu.id);
            }
        }, true);
    };

    blazorContextMenu.Show = function(menuId, x, y, target, triggerDotnetRef)
    {
        let targetId = null;
        if (target)
        {
            //add an id to the target dynamically so that it can be referenced later 
            //TODO: Rewrite this once this Blazor limitation is lifted
            if (!target.id)
                target.id = guid();
            
            targetId = target.id;
        }

        return menuHandlerReference.invokeMethodAsync('ShowMenu', menuId, x.toString(), y.toString(), targetId, triggerDotnetRef);
    }

    blazorContextMenu.Hide = function(menuId)
    {
        const menuElement = document.getElementById(menuId);
        const instanceId = menuElement.dataset["instanceId"];
        return menuHandlerReference.invokeMethodAsync('HideMenu', menuId).then(hideSuccessful =>
        {
            if (menuElement.classList.contains("blazor-context-menu") && hideSuccessful)
            {
                //this is a root menu. Remove from openMenus list
                const openMenu = openMenus.find((item) => item.instanceId === instanceId);
                if (openMenu)
                    removeItemFromArray(openMenus, openMenu);
            }
        });
    }

    blazorContextMenu.IsMenuShown = function(menuId)
    {
        const menuElement = document.getElementById(menuId);
        const instanceId = menuElement.dataset["instanceId"];
        const menu = openMenus.find(item => item.instanceId === instanceId);
        return typeof (menu) != 'undefined' && menu != null;
    }

    let subMenuTimeout = null;
    blazorContextMenu.OnMenuItemMouseOver = function(e, xOffset, currentItemElement)
    {
        //skip child menu mouseovers
        if (closest(e.target, ".blazor-context-menu__wrapper") !== closest(currentItemElement, ".blazor-context-menu__wrapper"))
            return;

        if (currentItemElement.getAttribute("itemEnabled") !== "true")
            return;

        //item does not contain a submenu
        const subMenu = findFirstChildByClass(currentItemElement, "blazor-context-submenu");
        if (!subMenu)
            return;

        subMenuTimeout = setTimeout(() =>
        {
            subMenuTimeout = null;

            const currentMenu = closest(currentItemElement, ".blazor-context-menu__wrapper");
            const currentMenuList = currentMenu.children[0];
            const rootMenu = closest(currentItemElement, ".blazor-context-menu");
            const targetRect = currentItemElement.getBoundingClientRect();
            const x = targetRect.left + currentMenu.clientWidth - xOffset;
            const y = targetRect.top;
            const instanceId = rootMenu.dataset["instanceId"];

            const openMenu = openMenus.find(item => item.instanceId === instanceId);

            blazorContextMenu.Show(subMenu.id, x, y, openMenu.target).then(function()
            {
                setOffset(subMenu);

                const closeSubMenus = function()
                {
                    const childSubMenus = findAllChildrenByClass(currentItemElement, "blazor-context-submenu");
                    let i = childSubMenus.length;
                    while (i--)
                    {
                        const subMenu = childSubMenus[i];
                        blazorContextMenu.Hide(subMenu.id);
                    }

                    i = currentMenuList.childNodes.length;
                    while (i--)
                    {
                        const child = currentMenuList.children[i];
                        if (child === currentItemElement)
                            continue;
                        child.removeEventListener("mouseover", closeSubMenus);
                    }
                };

                let i = currentMenuList.childNodes.length;
                while (i--)
                {
                    const child = currentMenuList.childNodes[i];
                    if (child === currentItemElement)
                        continue;
                    child.addEventListener("mouseover", closeSubMenus);
                }
            });
        }, 200);
    }

    blazorContextMenu.OnMenuItemMouseOut = function(args)
    {
        if (subMenuTimeout)
            clearTimeout(subMenuTimeout);
    }

    blazorContextMenu.RegisterTriggerReference = function(triggerElement, triggerDotNetRef)
    {
        if (triggerElement)
            triggerElement.dataset["dotnetref"] = JSON.stringify(triggerDotNetRef.serializeAsArg());
    }

    return blazorContextMenu;
}({});

blazorContextMenu.Init();