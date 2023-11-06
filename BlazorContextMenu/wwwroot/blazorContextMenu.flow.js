"use strict";

class DotNetRef {
    public serializeAsArg = function () { }
    public invokeMethodAsync = function () { }
}

class ContextMenu {
    target: Element;
    id: string;
    instanceId: string;
}

class blazorContextMenu
{
    // Constants
    public static WRAPPER : string = ".blazor-context-menu__wrapper";
    public static SUBMENU : string = "blazor-context-submenu";
    public static CONTEXT_MENU : string = "blazor-context-menu";
    public static INSTANCE_ID_KEY : string = "instanceId";
    
    private static showMenuCommon = function(menu, menuId : string, x, y, target : ContextMenu = null, triggerDotnetRef : DotNetRef = null) : void
    {
        return blazorContextMenu.Show(menuId, x, y, target, triggerDotnetRef).then(async () : Promise<void> =>
        {
            // Wait until the menu has spawned so clientWidth and offsetLeft report correctly
            await blazorContextMenu.sleepUntil(() : boolean => menu.clientWidth > 0, 1000);

            //check for overflow
            const leftOverflownPixels : number = menu.offsetLeft + menu.clientWidth - window.innerWidth;
            if (leftOverflownPixels > 0)
                menu.style.left = `${menu.offsetLeft - menu.clientWidth}px`;

            const topOverflownPixels : number = menu.offsetTop + menu.clientHeight - window.innerHeight;
            if (topOverflownPixels > 0)
                menu.style.top = `${menu.offsetTop - menu.clientHeight}px`;

            //openingMenu = false;
        });
    }
    
    private static openMenus : ContextMenu[] = [];

    public static closest : Function<HTMLElement> = null;
    /*if (window.Element && !Element.prototype.closest)
    {
        closest = (el : Element, s : string) : Element =>
        {
            let matches : NodeListOf<any> = (el.document || el.ownerDocument).querySelectorAll(s), i;
            do {
                i = matches.length;
                while (--i >= 0 && matches.item(i) !== el)
                {
    
                }
            } while ((i < 0) && (el = el.parentElement));
    
            return el;
        };
    }
    else
    {
        closest = (el : Element, s : string) : Element => el.closest(s);
    }*/

    //Helper functions
    //========================================
    private static guid() : string
    {
        function s4() : string
        {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }

    private static findFirstChildByClass(element : HTMLElement, className : string) : null
    {
        let foundElement : HTMLElement = null;

        function recurse(element : HTMLElement, className : string, found : boolean) : void
        {
            for (let i = 0; i < element.children.length && !found; i++)
            {
                const el : Element = element.children[i];
                
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

    private static findAllChildrenByClass(element : HTMLElement, className : string) : HTMLElement[]
    {
        const foundElements : HTMLElement[] = [];

        function recurse(element : HTMLElement, className : string) : void
        {
            for (let i = 0; i < element.children.length; i++)
            {
                const childElement : HTMLElement = element.children[i];
                if (childElement.classList.contains(className))
                    foundElements.push(element.children[i]);
                recurse(element.children[i], className);
            }
        }

        recurse(element, className);
        return foundElements;
    }

    private static removeItemFromArray(array : [], item : any) : void 
    {
        for (let i = 0; i < array.length; i++)
            if (array[i] === item)
                array.splice(i, 1);
    }

    private static sleepUntil = async (f : Function<boolean>, timeoutMs : number) : Promise<any> =>
    {
        return new Promise((resolve, reject) : void =>
        {
            const timeWas : Date = new Date();
            const wait : number = setInterval(() : void => 
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

    private static menuHandlerReference : DotNetRef = null;
    //var openingMenu = false;

    public static SetMenuHandlerReference = function(dotnetRef : DotNetRef) : void
    {
        if (!blazorContextMenu.menuHandlerReference) blazorContextMenu.menuHandlerReference = dotnetRef;
    }

    private static addToOpenMenus = function(menu : HTMLElement, menuId : string, target : ContextMenu = null) : void
    {
        const instanceId : string = blazorContextMenu.guid();

        blazorContextMenu.openMenus.push(
        {
            id: menuId,
            target: target,
            instanceId: instanceId
        });
        
        menu.dataset[blazorContextMenu.INSTANCE_ID_KEY] = instanceId;
    }

    public static ManualShow = function(menuId : string, x : number, y : number) : void
    {
        //openingMenu = true;
        const menu : HTMLElement = document.getElementById(menuId);
        if (!menu)
            throw new Error(`No context menu with id '${menuId}' was found`);

        blazorContextMenu.addToOpenMenus(menu, menuId);
        blazorContextMenu.showMenuCommon(menu, menuId, x, y);
    }

    public static OnContextMenu = function(e : MouseEvent, menuId : string, stopPropagation : boolean) : boolean
    {
        //openingMenu = true;
        const menu : HTMLElement = document.getElementById(menuId);
        if (!menu)
            throw new Error(`No context menu with id '${menuId}' was found`);

        blazorContextMenu.addToOpenMenus(menu, menuId, e.target);
        const triggerDotnetRef : DotNetRef = JSON.parse(e.currentTarget.dataset["dotnetref"]);
        blazorContextMenu.showMenuCommon(menu, menuId, e.x, e.y, e.target, triggerDotnetRef);
        e.preventDefault();
        if (stopPropagation)
            e.stopPropagation();
        return false;
    }

    public static Init = function() : void
    {
        document.addEventListener("mouseup", (e : MouseEvent) : void => handleAutoHideEvent(e, "mouseup"));

        document.addEventListener("mousedown", (e : MouseEvent) : void => handleAutoHideEvent(e, "mousedown"));

        function handleAutoHideEvent(e : MouseEvent, autoHideEvent : string) : void
        {
            if (blazorContextMenu.openMenus.length <= 0)
                return;
            
            for (let i = 0; i < blazorContextMenu.openMenus.length; i++)
            {
                const currentMenu : ContextMenu = blazorContextMenu.openMenus[i];
                const menuElement : HTMLElement = document.getElementById(currentMenu.id);
                if (menuElement && menuElement.dataset["autohide"] === "true" && menuElement.dataset["autohideevent"] === autoHideEvent)
                {
                    const clickedInsideMenu : boolean = menuElement.contains(e.target);
                    if (!clickedInsideMenu)
                        blazorContextMenu.Hide(currentMenu.id);
                }
            }
        }

        addEventListener('resize', () : void =>
        {
            if (blazorContextMenu.openMenus.length <= 0)
                return;
            
            for (let i = 0; i < blazorContextMenu.openMenus.length; i++)
            {
                const currentMenu : ContextMenu = blazorContextMenu.openMenus[i];
                const menuElement : HTMLElement = document.getElementById(currentMenu.id);
                if (menuElement && menuElement.dataset["autohide"] === "true")
                    blazorContextMenu.Hide(currentMenu.id);
            }
        }, true);
    }

    public static Show = function(menuId : string, x : number, y : number, target : ContextMenu, triggerDotnetRef : Object)
    {
        let targetId : string = null;
        if (target)
        {
            //add an id to the target dynamically so that it can be referenced later 
            //TODO: Rewrite this once this Blazor limitation is lifted
            if (!target.id)
                target.id = blazorContextMenu.guid();
            
            targetId = target.id;
        }

        return blazorContextMenu.menuHandlerReference.invokeMethodAsync('ShowMenu', menuId, x.toString(), y.toString(), targetId, triggerDotnetRef);
    }

    public static Hide = function(menuId : string)
    {
        const menuElement : HTMLElement = document.getElementById(menuId);
        const instanceId : (string | undefined) = menuElement.dataset[blazorContextMenu.INSTANCE_ID_KEY];
        return blazorContextMenu.menuHandlerReference.invokeMethodAsync('HideMenu', menuId).then((hideSuccessful : boolean) : void =>
        {
            if (menuElement.classList.contains(blazorContextMenu.CONTEXT_MENU) && hideSuccessful)
            {
                //this is a root menu. Remove from openMenus list
                const openMenu : ContextMenu = blazorContextMenu.openMenus.find((item : ContextMenu) : boolean => item.instanceId === instanceId);
                if (openMenu)
                    blazorContextMenu.removeItemFromArray(blazorContextMenu.openMenus, openMenu);
            }
        });
    }

    public static IsMenuShown = function(menuId : string)
    {
        const menuElement : HTMLElement = document.getElementById(menuId);
        const instanceId : (string | undefined) = menuElement.dataset[blazorContextMenu.INSTANCE_ID_KEY];
        const menu : ContextMenu = blazorContextMenu.openMenus.find((item : ContextMenu) : boolean => item.instanceId === instanceId);
        return typeof (menu) != 'undefined' && menu != null;
    }

    private static subMenuTimeout : Function = null;
    public static OnMenuItemMouseOver = function(e : MouseEvent, xOffset : number, currentItemElement : Element) : void
    {
        //skip child menu mouseovers
        if (blazorContextMenu.closest(e.target, blazorContextMenu.WRAPPER) !== blazorContextMenu.closest(currentItemElement, blazorContextMenu.WRAPPER))
            return;

        if (currentItemElement.getAttribute("itemEnabled") !== "true")
            return;

        const subMenu = blazorContextMenu.findFirstChildByClass(currentItemElement, blazorContextMenu.SUBMENU);
        if (!subMenu)
            return; //item does not contain a submenu

        blazorContextMenu.subMenuTimeout = setTimeout(() : void =>
        {
            blazorContextMenu.subMenuTimeout = null;

            const currentMenu = blazorContextMenu.closest(currentItemElement, blazorContextMenu.WRAPPER);
            const currentMenuList : Element = currentMenu.children[0];
            const rootMenu : HTMLElement = blazorContextMenu.closest(currentItemElement, blazorContextMenu.CONTEXT_MENU);
            const targetRect : DOMRect = currentItemElement.getBoundingClientRect();
            const x : number = targetRect.left + currentMenu.clientWidth - xOffset;
            const y : number = targetRect.top;
            const instanceId : string = rootMenu.dataset[blazorContextMenu.INSTANCE_ID_KEY];

            const openMenu : ContextMenu = blazorContextMenu.openMenus.find((item : ContextMenu) : boolean => item.instanceId === instanceId);

            blazorContextMenu.Show(subMenu.id, x, y, openMenu.target).then(() : void =>
            {
                const leftOverflownPixels : number = subMenu.offsetLeft + subMenu.clientWidth - window.innerWidth;
                if (leftOverflownPixels > 0)
                    subMenu.style.left = `${subMenu.offsetLeft - subMenu.clientWidth - currentMenu.clientWidth - xOffset}px`

                const topOverflownPixels : number = subMenu.offsetTop + subMenu.clientHeight - window.innerHeight;
                if (topOverflownPixels > 0)
                    subMenu.style.top = `${subMenu.offsetTop - topOverflownPixels}px`;

                const closeSubMenus = function() : void
                {
                    const childSubMenus : [] = blazorContextMenu.findAllChildrenByClass(currentItemElement, blazorContextMenu.SUBMENU);
                    
                    let i : number = childSubMenus.length;
                    while (i--)
                    {
                        const subMenu = childSubMenus[i];
                        blazorContextMenu.Hide(subMenu.id);
                    }

                    i = currentMenuList.childNodes.length;
                    while (i--)
                    {
                        const child : Element = currentMenuList.children[i];
                        if (child === currentItemElement)
                            continue;
                        
                        child.removeEventListener("mouseover", closeSubMenus);
                    }
                };

                let i : number = currentMenuList.childNodes.length;
                while (i--)
                {
                    const child : ChildNode = currentMenuList.childNodes[i];
                    
                    if (child === currentItemElement)
                        continue;
                    
                    child.addEventListener("mouseover", closeSubMenus);
                }
            });
        }, 200);
    }

    public static OnMenuItemMouseOut = function(e : MouseEvent) : void
    {
        if (blazorContextMenu.subMenuTimeout)
            clearTimeout(blazorContextMenu.subMenuTimeout);
    }

    public static RegisterTriggerReference = function(triggerElement : HTMLElement, triggerDotNetRef : DotNetRef) : void
    {
        if (triggerElement)
            triggerElement.dataset["dotnetref"] = JSON.stringify(triggerDotNetRef.serializeAsArg());
    }
}

blazorContextMenu.Init();