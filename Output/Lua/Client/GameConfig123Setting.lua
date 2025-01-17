local GameConfig123Setting={
	{id=1,key="TURN_TIMES",value="10"},


	-- 键映射
	k = {
		id = "id",-- 主键
		key = "key",-- 键
		value = "value",-- 值
	}
}

local cacheMap = {}

function GameConfig123Setting:Get(key, value)
	local mapKey = "GameConfig123Setting".."_"..key.."_"..value
	local index = rawget(cacheMap, mapKey) or nil
	if index then
		if index > 0 then
			return GameConfigSetting[index]
		else
			return nil
		end
	end
	for i,vo in ipairs(GameConfig123Setting) do
		for k,v in pairs(vo) do
			if k == key and v == value then
				rawset(cacheMap, mapKey, i)
				return GameConfig123Setting[i]
			end
		end
	end

	rawset(cacheMap, mapKey, -1)
	return nil
end

return GameConfig123Setting