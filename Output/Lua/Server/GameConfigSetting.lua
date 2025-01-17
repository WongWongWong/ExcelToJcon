local GameConfigSetting={
	{id=1,key="TURN_TIMES"},
	{id=2,key="TURN_TIMES111"},
	{id=3,key="asd"},
	{id=1,key="Test1",TestValue="hahaha"},
	{id=1,key="Test2"},


	-- 键映射
	k = {
		id = "id",-- 主键
		key = "key",-- 键
		TestValue = "TestValue",-- 测试值
	}
}

local cacheMap = {}

function GameConfigSetting:Get(key, value)
	local mapKey = "GameConfigSetting".."_"..key.."_"..value
	local index = rawget(cacheMap, mapKey) or nil
	if index then
		if index > 0 then
			return GameConfigSetting[index]
		else
			return nil
		end
	end
	for i,vo in ipairs(GameConfigSetting) do
		for k,v in pairs(vo) do
			if k == key and v == value then
				rawset(cacheMap, mapKey, i)
				return GameConfigSetting[i]
			end
		end
	end

	rawset(cacheMap, mapKey, -1)
	return nil
end

return GameConfigSetting